using UnityEngine;

public class Floater : MonoBehaviour
{
    [SerializeField] private Rigidbody boatBody;
    [SerializeField] private bool isSubmerged;
    [SerializeField] float displacementAmount = 3f;
    [SerializeField] private int floaterCount = 1;
    [SerializeField] private float waterDrag = 0.99f;
    [SerializeField] private AnimationCurve dragCurve;
    [SerializeField] [Range(0f, 0.5f)]private float floaterMass = .2f;
    
    private void FixedUpdate()
    {
        Vector3 floaterPosition = transform.position;
        
        boatBody.AddForceAtPosition((Physics.gravity / floaterCount) * boatBody.mass, floaterPosition);
        //find point
        float waveHeight = WaveManager.Instance.FindPoint(new Vector2(floaterPosition.x, floaterPosition.z)).y;
        if (floaterPosition.y < waveHeight)
        {
            /*float displacementMultiplier = Mathf.Clamp01((waveHeight - transform.position.y) / depthBeforeSubmerged) * displacementAmount;
            //Pushes the floating point above waveheight
            boatBody.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0f), transform.position, ForceMode.Acceleration);
            //Need to look into this part so the boat can glide through the waves when moving
            //Adds drag to the floating point //Maybe less drag when moving?
            boatBody.AddForce(displacementMultiplier * (-boatBody.velocity * waterDrag) * Time.fixedDeltaTime, ForceMode.VelocityChange);
            boatBody.AddTorque(displacementMultiplier * (-boatBody.angularVelocity * waterAngularDrag) * Time.fixedDeltaTime, ForceMode.VelocityChange);*/
            
            //Bouyancy - Y Axis
            //Make it so the boat tries to get to the wave height and then stops 
            Vector3 surfaceDirection = Vector3.up;

            Vector3 floaterWorldVelocity = boatBody.GetPointVelocity(floaterPosition);
            
            //Calculate the offset (distance between waveHeight and floater position)
            float offset = Mathf.Clamp01(waveHeight - floaterPosition.y);
            
            //Calculate velocity along the up axis of the floater
            float velocity = Vector3.Dot(surfaceDirection, floaterWorldVelocity);
            
            float force = boatBody.mass * (offset * displacementAmount - velocity * waterDrag);
            
            if (!isSubmerged)
            {
                boatBody.AddForceAtPosition(surfaceDirection * force, floaterPosition);
            }

            //Bouyancy - Drag/Steering
            Vector3 steeringDirection = transform.right;
            float steeringVelocity = Vector3.Dot(steeringDirection, floaterWorldVelocity);
            /*Debug.Log("Velocity : " + steeringVelocity);
            Debug.Log("Drag : " + dragCurve.Evaluate(Mathf.Abs(steeringVelocity)));*/
            float desiredVelocityChange = boatBody.mass * (-steeringVelocity * dragCurve.Evaluate(Mathf.Abs(steeringVelocity)));
            float desiredAcceleration = desiredVelocityChange / Time.fixedDeltaTime;
            
            //Apply drag force
            boatBody.AddForceAtPosition(steeringDirection * floaterMass * desiredAcceleration, floaterPosition);
        }
    }

    public void SetDragCurve(AnimationCurve curve)
    {
        dragCurve = curve;
    }
}
