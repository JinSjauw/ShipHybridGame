using System;
using UnityEngine;

public class NetController : MonoBehaviour
{
    [SerializeField] private Rigidbody netBody;
    [SerializeField] private Transform anchor;
    [SerializeField] private Transform hook;

    [SerializeField] private float targetLoweredRotation;
    
    
    private bool hookLowered;
    // Have the ball follow an anchor;

    private void FixedUpdate()
    {
        netBody.MovePosition(anchor.position);
    }

    private void RotateHook(bool state)
    {
        //Lerp hook towards target rotation
    }
    
    public void ActuateHook()
    {
        
    }
    
    
}
