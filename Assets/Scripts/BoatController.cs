using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class BoatController : MonoBehaviour
{
    [Header("Boat References")] 
    [SerializeField] private Transform engineTransform;
    [SerializeField] private List<Transform> steeringPoints;
    [SerializeField] private List<Transform> accelerationPoints;
    [SerializeField] private List<Floater> floaters;
    [SerializeField] private Transform nose;
    [SerializeField] private bool showForces;
    
    [Header("Boat Locomotion")]
    
    [Header("Boat Acceleration")]
    [SerializeField] private float boatTopSpeed;
    [SerializeField] private AnimationCurve powerCurve;
    /*[SerializeField] private float accelerationDelta;
    [SerializeField] private AnimationCurve accelerationCurve;*/
    
    [Header("Boat Steering")]
    [SerializeField] private float turnSpeed;
    [SerializeField] private float maxTurnAngle = 40;
    [SerializeField] private AnimationCurve frontDragCurve;
    [SerializeField] private AnimationCurve backDragCurve;

    [Header("Boat Buoyancy")] 
    [SerializeField] private float springStrength;
    [SerializeField] private float damperStrength;
    
    private Rigidbody boatBody;
    
    private bool isMoving;
    private bool isTurning;
    private bool isDrifting;
    
    private float throttle;
    private float boatSpeed;
    private float normalizedSpeed;
    
    private float turnRate;
    private float turnAngle;

    // Start is called before the first frame update
    void Start()
    {
        boatBody = GetComponent<Rigidbody>();

        foreach (Floater floater in floaters)
        {
            floater.SetDragCurve(frontDragCurve);
            floater.SetFrictionCurve(frontDragCurve);
        }

        foreach (Transform accelPoint in accelerationPoints)
        {
            if (accelPoint.TryGetComponent(out Floater floater))
            {
                floater.SetFrictionCurve(backDragCurve);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        KeepUpright();
        
        //Debugging Forces
        
        //Debug.DrawRay(nose.transform.position, boatBody.velocity, Color.blue);

        boatSpeed = Vector3.Dot(transform.forward, boatBody.velocity);
        normalizedSpeed = Mathf.Clamp01(Mathf.Abs(boatSpeed / boatTopSpeed));
        
        if (throttle != 0 && WaterCheck())
        {
            if (normalizedSpeed >= .99f)
            {
                return;
            }
            
            float availablePower = powerCurve.Evaluate(normalizedSpeed) * throttle * boatBody.mass;
    
            foreach (Transform point in accelerationPoints)
            {
                Vector3 accelDirection = point.forward;
                /*if (isDrifing)
                {
                    accelDirection = Quaternion.AngleAxis(turnAngle * turnRate, Vector3.up) * accelDirection;
                }*/
                accelDirection.y = 0;
                /*Debug.Log("BoatSpeed " + boatSpeed + " Normalized Speed: " + normalizedSpeed);
                Debug.Log("Acceleration: " + availablePower);*/
                boatBody.AddForceAtPosition(accelDirection * availablePower * 10f, point.position);
                DrawArrow.ForDebug(nose.position, accelDirection * availablePower * 10f, Color.yellow);
            }
        }
        
        if (isTurning)
        {
            //Applies torque;
            /*if (Mathf.Abs(turnAngle + turnRate) < maxTurnAngle)
            {
                float angleChange = turnRate * turnSpeed * Time.fixedDeltaTime;
                turnAngle += angleChange;
                turnAngle = Mathf.Clamp(turnAngle, -maxTurnAngle, maxTurnAngle);

                Vector3 angularVelocity = new Vector3(0, turnAngle, 0);
                Quaternion deltaRotation = Quaternion.Euler(boatBody.mass * angularVelocity * Time.fixedDeltaTime);
                boatBody.MoveRotation(boatBody.rotation * deltaRotation);
            }*/
            
            
            //Turns the steering point;
            /*foreach (Transform point in steeringPoints)
            {
                if (Mathf.Abs(turnAngle + turnRate) < maxTurnAngle)
                {
                    float angleChange = turnRate * turnSpeed * Time.fixedDeltaTime;
                    turnAngle += angleChange;
                    turnAngle = Mathf.Clamp(turnAngle, -maxTurnAngle, maxTurnAngle);
                    point.localRotation = Quaternion.Euler(0, turnAngle, 0);
                }
            }*/
        }

        if (isDrifting)
        {
            float force = boatBody.mass * 35f;
            float torque = boatBody.mass * turnAngle;
            //boatBody.AddTorque(transform.up * torque * turnRate);
            //boatBody.AddForce(boatBody.transform.right * force);
            
            foreach (Floater floater in floaters)
            {
                floater.UseDriftCurve(true);
            }
            
            if (showForces)
            {
                DrawArrow.ForDebug(transform.position, boatBody.transform.right * force * .3f, Color.magenta);
            }
        }
        
        if (showForces)
        {
            DrawArrow.ForDebug(nose.transform.position, boatBody.velocity.normalized * 3f * normalizedSpeed, Color.blue);
        }
    }

    private void KeepUpright()
    {
        float offset = Mathf.Clamp01(1f - Vector3.Dot(Vector3.up, boatBody.transform.up));
        
        //Debug.Log("Deck Offset: " + Mathf.Clamp01(1f - offset));
        
        Vector3 springTorque = boatBody.mass * (offset * springStrength) * Vector3.Cross(boatBody.transform.up, Vector3.up);
        Vector3 dampTorque = boatBody.mass * (damperStrength) * boatBody.angularVelocity;

        boatBody.AddTorque(springTorque - dampTorque);
    }
    
    private bool WaterCheck()
    {
        Vector3 enginePosition = engineTransform.position;
        if (WaveManager.Instance.FindPoint(new Vector2(enginePosition.x, enginePosition.z)).y > enginePosition.y)
        {
            return true;
        }
        
        return false;
    }
    
    public void OnThrust(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Debug.Log("Moving: " + context.ReadValue<Vector2>());
            throttle = context.ReadValue<float>();
            //boatBody.AddForce(context.ReadValue<float>() * boatBody.transform.right * moveSpeed, ForceMode.Acceleration);
        }

        if (context.canceled)
        {
            throttle = 0;
        }
    }

    public void OnSteer(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            turnRate = context.ReadValue<float>();
            //Debug.Log(turnRate);
            if (turnRate != 0)
            {
                isTurning = true;

                if (isDrifting)
                {
                    turnAngle = 0;
                    foreach (Transform point in steeringPoints)
                    {
                        point.localRotation = Quaternion.Euler(0,0,0);
                    }
                }
            }
        }

        if (context.canceled)
        {
            turnRate = 0;
            isTurning = false;
            if (!isDrifting)
            {
                turnAngle = 0;
                foreach (Transform point in steeringPoints)
                {
                    point.localRotation = Quaternion.Euler(0,0,0);
                }
            }
        }
    }

    public void OnDrift(InputAction.CallbackContext context)
    {
        //Check the steering input and acceleration.
        
        //#Core points for drifting
        
        //Apply a force to the side of the rigidbody (depending on steering input)
        //In addition to the forward force

        if (context.started)
        {
            isDrifting = true;
        }

        if (context.canceled)
        {
            isDrifting = false;
            foreach (Floater floater in floaters)
            {
                floater.UseDriftCurve(false);
            }
            turnAngle = 0;
            foreach (Transform point in steeringPoints)
            {
                point.localRotation = Quaternion.Euler(0,0,0);
            }
        }
    }
}
