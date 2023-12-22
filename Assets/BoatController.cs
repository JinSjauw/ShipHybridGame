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
        
        
        if (throttle != 0 && WaterCheck())
        {
            Vector3 velocity = throttle * boatBody.transform.forward * moveSpeed * Time.deltaTime;
            boatBody.transform.Rotate(Vector3.up, turnRate);
            //velocity = Quaternion.AngleAxis(turnAngle, Vector3.up) * velocity;
            boatBody.AddForce(velocity, ForceMode.VelocityChange);
        }
        
        //Rotate Rudder (if turnangle changed)
        /*if (turnRate != 0)
        {
            if (Mathf.Abs(turnAngle) < maxTurnAngle)
            {
                Debug.Log("Changing Direction:" + turnRate + " : " + turnAngle);
                turnAngle += turnRate * turnSpeed * Time.deltaTime;
            }
            rudderTransform.localRotation = Quaternion.AngleAxis(turnAngle, Vector3.up);
        }*/
    }

    private bool WaterCheck()
    {
        if (Physics.Raycast(boatBody.position, Vector3.down, 10f, LayerMask.GetMask("Water")))
        {
            return true;
        }
        
        return false;
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Moving: " + context.ReadValue<Vector2>());
            throttle = context.ReadValue<Vector2>().y;
            turnRate = context.ReadValue<Vector2>().x;
            //boatBody.AddForce(context.ReadValue<float>() * boatBody.transform.right * moveSpeed, ForceMode.Acceleration);
        }

        if (context.canceled)
        {
            throttle = 0;
        }
    }
}
