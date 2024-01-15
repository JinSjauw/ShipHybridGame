using UnityEngine;

public class NetController : MonoBehaviour
{
    [SerializeField] private Rigidbody netBody;
    [SerializeField] private Transform anchor;
    [SerializeField] private Transform hook;

    [SerializeField] private AnimationCurve hookRotationCurve;
    [SerializeField] private float actionSpeed = 3;
    
    [SerializeField] private float loweredRotation;
    [SerializeField] private float neutralRotation;
    
    private bool hookLowered;
    private bool rotatingHook;

    private Quaternion fromRotation;
    private Quaternion targetRotation;

    private float current;
    
    // Have the ball follow an anchor;

    private void FixedUpdate()
    {
        netBody.MovePosition(anchor.position);

        if (rotatingHook)
        {
            RotateHook(fromRotation, targetRotation);
        }
    }

    private void RotateHook(Quaternion from, Quaternion to)
    {
        //Increment variable
        current = Mathf.MoveTowards(current, 1, actionSpeed * Time.fixedDeltaTime);
        
        //Lerp hook towards target rotation
        hook.localRotation = Quaternion.Lerp(from, to, hookRotationCurve.Evaluate(current));

        //When alpha is 1, rotatingHook = false;
        if (current >= 1)
        {
            current = 0;
            rotatingHook = false;
            hookLowered = !hookLowered;
        }
    }
    
    public void ActuateHook()
    {
        if (rotatingHook) return;
        
        float targetValue = hookLowered ? neutralRotation : loweredRotation;
        targetRotation = Quaternion.AngleAxis(targetValue, Vector3.right);
        targetRotation *= Quaternion.AngleAxis(-90, Vector3.forward);
        fromRotation = hook.localRotation;
        
        rotatingHook = true;
    }

    public bool IsHookLowered()
    {
        return hookLowered;
    }
    
    
}
