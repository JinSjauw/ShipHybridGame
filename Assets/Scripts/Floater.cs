using UnityEngine;

public class Floater : MonoBehaviour
{
    [SerializeField] private Rigidbody boatBody;
    [SerializeField] private bool isSubmerged;
    [SerializeField] float displacementAmount = 3f;
    /*[SerializeField] private int floaterCount = 1;
    [SerializeField] private float waterDrag = 0.99f;*/
    [SerializeField] private AnimationCurve dragCurve;
    [SerializeField] private AnimationCurve frictionCurve;
    [SerializeField] private AnimationCurve driftingCurve;
    [SerializeField] [Range(0, .25f)]private float floaterMass = .2f;

    [SerializeField] private bool debugCurve;
    [SerializeField] private bool showForces = true;
    private float frictionVelocity;
    private Vector3 steeringDir;
    private bool isDrifting;
    
    private void FixedUpdate()
    {
        Vector3 floaterPosition = transform.position;
        
        boatBody.AddForceAtPosition((Physics.gravity) * boatBody.mass, floaterPosition);

        //find point
        float waveHeight = WaveManager.Instance.FindPoint(new Vector2(floaterPosition.x, floaterPosition.z)).y;
        if (floaterPosition.y < waveHeight)
        {
            //Bouyancy - Y Axis
            //Make it so the boat tries to get to the wave height and then stops 
            Vector3 surfaceDirection = Vector3.up;

            Vector3 floaterWorldVelocity = boatBody.GetPointVelocity(floaterPosition);
            
            //Calculate the offset (distance between waveHeight and floater position)
            float offset = Mathf.Clamp01(waveHeight - floaterPosition.y);
            
            //Calculate velocity along the up axis of the floater
            float velocity = Vector3.Dot(surfaceDirection, floaterWorldVelocity);
            
            float force = boatBody.mass * (offset * displacementAmount - velocity * dragCurve.Evaluate(velocity));
            
            if (!isSubmerged)
            {
                boatBody.AddForceAtPosition(surfaceDirection * force, floaterPosition);
            }
            
            //Bouyancy - Drag/Steering
            Vector3 steeringDirection = transform.right;
            float steeringVelocity = Vector3.Dot(steeringDirection, floaterWorldVelocity);
            
            if (debugCurve)
            {
                Debug.Log("Velocity : " + steeringVelocity);
                //Debug.Log("Drag : " + driftingCurve.Evaluate(Mathf.Abs(steeringVelocity)));
            }

            float desiredVelocityChange = (-steeringVelocity * frictionCurve.Evaluate(Mathf.Abs(steeringVelocity)));

            if (isDrifting)
            {
                desiredVelocityChange = (-steeringVelocity * driftingCurve.Evaluate(Mathf.Abs(steeringVelocity)));
            }
            
            
            float desiredAcceleration = desiredVelocityChange / Time.fixedDeltaTime;
           
            steeringDir = steeringDirection;
            frictionVelocity = desiredAcceleration;
            
            //Apply drag force
            boatBody.AddForceAtPosition(boatBody.mass * (steeringDirection * floaterMass * desiredAcceleration), floaterPosition);
        }
        
        if (showForces)
        {
            DrawArrow.ForDebug(transform.position, (steeringDir * floaterMass * frictionVelocity), Color.red);
        }
    }

    public void SetDragCurve(AnimationCurve curve)
    {
        dragCurve.CopyFrom(curve);
        //debugCurve = true;
    }

    public void SetFrictionCurve(AnimationCurve curve)
    {
        frictionCurve.CopyFrom(curve);
    }

    public void UseDriftCurve(bool state)
    {
        isDrifting = state;
    }
}
