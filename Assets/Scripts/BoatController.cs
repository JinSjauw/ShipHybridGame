using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class BoatController : MonoBehaviour
{
    [Header("Boat Physics References")]

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
    [SerializeField] private AnimationCurve dragCurve;
    [SerializeField] private AnimationCurve frictionCurve;

    [Header("Boat Buoyancy")]
    [SerializeField] private float springStrength;
    [SerializeField] private float damperStrength;

    [Header("Boat VFX")]

    [SerializeField] private float sidewaysVelocity;
    [SerializeField] private ParticleSystem leftDriftParticle;
    [SerializeField] private ParticleSystem rightDriftParticle;
    [SerializeField] private ParticleSystem wakeParticle;
    [SerializeField] private float minStartDriftVelocity;
    [SerializeField] private float minEndDriftVelocity;

    private Rigidbody boatBody;
    public Transform boneToRotate;
    private bool isMoving;
    private bool isTurning;
    private bool isDrifting;

    private float throttle;
    private float boatSpeed;
    private float normalizedSpeed;

    private float turnRate;
    private float turnAngle;

    [Header("UI")]
    public Transform wheel;

    // Start is called before the first frame update
    void Start()
    {
        boatBody = GetComponent<Rigidbody>();

        foreach (Floater floater in floaters)
        {
            //floater.SetDragCurve(backDragCurve);
            floater.SetDragCurve(dragCurve);
        }

        foreach (Transform accelPoint in accelerationPoints)
        {
            if (accelPoint.TryGetComponent(out Floater floater))
            {
                floater.SetFrictionCurve(frictionCurve);
            }
        }
    }


    void RotateBoneBasedOnValue(float rotationValue)
    {
        Quaternion rotation = Quaternion.Euler(rotationValue + 180, 0f, 0f);
        boneToRotate.localRotation = rotation;
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        KeepUpright();


        //Debugging Forces

        //Debug.DrawRay(nose.transform.position, boatBody.velocity, Color.blue);

        DisplayParticles();
        // visualises rudder rotation
        RotateRudder();
        Centerwheel();
        //Acceleration
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
                accelDirection.y = 0;
                Vector3 forwardForce = accelDirection * availablePower * 10f;
                boatBody.AddForceAtPosition(forwardForce, point.position);
                DrawArrow.ForDebug(point.position, accelDirection * availablePower, Color.yellow);
            }

            /*Vector3 forwardForce = transform.forward * availablePower * 10f;
            boatBody.AddForce(forwardForce);*/
        }

        if (isTurning)
        {
            //Applies torque;
            if (Mathf.Abs(turnAngle + turnRate) < maxTurnAngle)
            {
                float angleChange = turnRate * turnSpeed * Time.fixedDeltaTime;
                turnAngle += angleChange;
                turnAngle = Mathf.Clamp(turnAngle, -maxTurnAngle, maxTurnAngle);

                Vector3 angularVelocity = new Vector3(0, turnAngle, 0);
                //Quaternion deltaRotation = Quaternion.Euler(turnSpeed * angularVelocity * Time.fixedDeltaTime);

                boatBody.angularVelocity = (turnSpeed * angularVelocity) * normalizedSpeed;
            }
        }

        if (showForces)
        {
            DrawArrow.ForDebug(nose.transform.position, boatBody.velocity.normalized * 3f * normalizedSpeed, Color.blue);
        }


    }

    private void RotateRudder()
    {
        float normalizedRotation = Mathf.InverseLerp(-40, 40, turnAngle);
        float remappedRotation = Mathf.Lerp(-90, 90, normalizedRotation);
        RotateBoneBasedOnValue(remappedRotation);
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

    private void Centerwheel()
    {
        float normalizedRotation1 = Mathf.InverseLerp(-40, 40, turnAngle);
        float remappedRotation1 = Mathf.Lerp(360, -360, normalizedRotation1);
        wheel.transform.rotation = Quaternion.Euler(0f, 0f, remappedRotation1);
    }
    private void DisplayParticles()
    {
        //Debug.Log("Sidewards Velocity: " + Vector3.Dot(boatBody.velocity, transform.right));
        float velocity = SidewaysVelocity();

        float normalizedVelocity = Mathf.InverseLerp(7, 30, Mathf.Abs(velocity));
        float remappedVelocity = Mathf.Lerp(0.1f, 1f, normalizedVelocity);

        if (throttle != 0 && WaterCheck())
        {
            if (!wakeParticle.isPlaying)
            {
                wakeParticle.Play();
            }

            if (Mathf.Abs(velocity) > minStartDriftVelocity)
            {
                switch (velocity)
                {
                    case > 0:
                        rightDriftParticle.Play();
                        rightDriftParticle.startSize = remappedVelocity;
                        break;
                    case < 0:
                        leftDriftParticle.Play();
                        leftDriftParticle.startSize = remappedVelocity;
                        break;
                }

                isDrifting = true;
            }
        }
        else
        {
            wakeParticle.Stop();
        }

        if (Mathf.Abs(velocity) <= minEndDriftVelocity)
        {
            if (rightDriftParticle.isPlaying)
            {
                rightDriftParticle.Stop();
            }

            if (leftDriftParticle.isPlaying)
            {
                leftDriftParticle.Stop();
            }

            isDrifting = false;
        }
    }

    private float SidewaysVelocity()
    {
        return Vector3.Dot(boatBody.velocity, transform.right);
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
                        point.localRotation = Quaternion.Euler(0, 0, 0);
                    }
                }
            }
        }

        if (context.canceled)
        {
            turnRate = 0;
            turnAngle = 0;
            isTurning = false;

            /*if (!isDrifting)
            {
                turnAngle = 0;
                foreach (Transform point in steeringPoints)
                {
                    point.localRotation = Quaternion.Euler(0,0,0);
                }
            }*/
        }
    }

    public void OnDrift(InputAction.CallbackContext context)
    {
        //Check the steering input and acceleration.

        //#Core points for drifting

        //Apply a force to the side of the rigidbody (depending on steering input)
        //In addition to the forward force

        /*if (context.started)
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
        }*/
    }
}
