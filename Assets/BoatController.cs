using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class BoatController : MonoBehaviour
{
    [SerializeField] private List<Floater> _floaters;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform rudderTransform;

    [SerializeField] private float turnSpeed;
    [SerializeField] private float maxTurnAngle = 40;
    
    private Rigidbody boatBody;
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
        if (throttle != 0)
        {
            Vector3 velocity = throttle * boatBody.transform.right * moveSpeed * Time.deltaTime;
            velocity = Quaternion.AngleAxis(turnAngle, Vector3.up) * velocity;
            boatBody.AddForce(velocity, ForceMode.Acceleration);
        }
        
        //Rotate Rudder (if turnangle changed)
        if (turnRate != 0)
        {
            if (Mathf.Abs(turnAngle) < maxTurnAngle)
            {
                Debug.Log("Changing Direction:" + turnRate + " : " + turnAngle);
                turnAngle += turnRate * turnSpeed * Time.deltaTime;
            }
            rudderTransform.localRotation = Quaternion.AngleAxis(turnAngle, Vector3.up);
        }
    }

    public void OnThrottle(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Thrusting: " + context.ReadValue<float>());
            throttle = context.ReadValue<float>();
            //boatBody.AddForce(context.ReadValue<float>() * boatBody.transform.right * moveSpeed, ForceMode.Acceleration);
        }

        if (context.canceled)
        {
            throttle = 0;
        }
    }
    
    public void OnRudder(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            turnRate = context.ReadValue<float>();
        }

        if (context.canceled)
        {
            turnRate = 0;
        }
    }
}
