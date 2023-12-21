using System;
using UnityEngine;

public class Floater : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] float depthBeforeSubmerged = 1f;
    [SerializeField] float displacementAmount = 3f;
    [SerializeField] private int floaterCount = 1;
    [SerializeField] private float waterDrag = 0.99f;
    [SerializeField] private float waterAngularDrag = 0.5f;
    
    private void FixedUpdate()
    {
        rigidbody.AddForceAtPosition(Physics.gravity / floaterCount, transform.position, ForceMode.Acceleration);
        
        //find point
        float waveHeight = WaveManager.Instance.FindPoint(new Vector2(transform.position.x, transform.position.z)).y;
        if (transform.position.y < waveHeight)
        {
            float displacementMultiplier = Mathf.Clamp01((waveHeight - transform.position.y) / depthBeforeSubmerged) * displacementAmount;
            rigidbody.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0f), transform.position, ForceMode.Acceleration);
            rigidbody.AddForce(displacementMultiplier * -rigidbody.velocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
            rigidbody.AddTorque(displacementMultiplier * -rigidbody.angularVelocity * waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }
}
