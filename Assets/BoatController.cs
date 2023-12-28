using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class BoatController : MonoBehaviour
{
    [FormerlySerializedAs("enginePosition")]
    [Header("Boat References")] 
    [SerializeField] private Transform engineTransform;
    [SerializeField] private List<Transform> steeringPoints;
    [SerializeField] private List<Floater> drivingPoints;

    [Header("Boat Movement Variables")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float maxTurnAngle = 40;
    
    private Rigidbody boatBody;

    private bool isMoving;
    private bool isTurning;
    
    private float throttle;
    private float turnRate;
    private float turnAngle;

    // Start is called before the first frame update
    void Start()
    {
        boatBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (throttle != 0 && WaterCheck())
        {
            Vector3 velocity = throttle * boatBody.transform.forward * moveSpeed * Time.deltaTime;
            //boatBody.transform.Rotate(Vector3.up, turnRate * turnSpeed);
            //boatBody.AddTorque(boatBody.transform.up * (turnRate * turnSpeed), ForceMode.VelocityChange);
            //velocity = Quaternion.AngleAxis(turnAngle, Vector3.up) * velocity;

            boatBody.AddForce(velocity, ForceMode.Acceleration);
        }

        if (isTurning)
        {
            foreach (Transform point in steeringPoints)
            {
                Debug.Log(turnAngle);
                if (Mathf.Abs(turnAngle + turnRate) < maxTurnAngle)
                {
                    float angleChange = turnRate * turnSpeed * Time.fixedDeltaTime;
                    turnAngle += angleChange;
                    turnAngle = Mathf.Clamp(turnAngle, -maxTurnAngle, maxTurnAngle);
                    //point.rotation *= Quaternion.AngleAxis(angleChange, Vector3.up);
                    point.localRotation = Quaternion.Euler(0, turnAngle, 0);
                }
            }
        }
        
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
    
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Debug.Log("Moving: " + context.ReadValue<Vector2>());
            throttle = context.ReadValue<Vector2>().y;
            turnRate = context.ReadValue<Vector2>().x;

            if (turnRate != 0)
            {
                isTurning = true;
            }
            //boatBody.AddForce(context.ReadValue<float>() * boatBody.transform.right * moveSpeed, ForceMode.Acceleration);
        }

        if (context.canceled)
        {
            throttle = 0;
            turnRate = 0;
            isTurning = false;
        }
    }
}
