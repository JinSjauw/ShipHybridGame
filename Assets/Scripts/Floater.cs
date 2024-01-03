using UnityEngine;

public class Floater : MonoBehaviour
{
    [SerializeField] private Rigidbody boatBody;
    [SerializeField] private bool isSubmerged;
    [SerializeField] float displacementAmount = 3f;
    [SerializeField] private int floaterCount = 1;
    [SerializeField] private float waterDrag = 0.99f;
    [SerializeField] private AnimationCurve dragCurve;
    [SerializeField] [Range(0, .25f)]private float floaterMass = .2f;

    [SerializeField] private bool debugCurve;
    
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
                Debug.Log("Drag : " + dragCurve.Evaluate(Mathf.Abs(steeringVelocity)));
            }
            
            float desiredVelocityChange = (-steeringVelocity * dragCurve.Evaluate(Mathf.Abs(steeringVelocity)));
            float desiredAcceleration = desiredVelocityChange / Time.fixedDeltaTime;
            
            //Apply drag force
            boatBody.AddForceAtPosition(boatBody.mass * (steeringDirection * floaterMass * desiredAcceleration), floaterPosition);
        }
    }

    public void SetDragCurve(AnimationCurve curve)
    {
        dragCurve = curve;
        //debugCurve = true;
    }
}
