using TheKiwiCoder;
using UnityEngine;

public class ChargeAttack : ActionNode
{
    public AnimationCurve depthCurve;
    public float chargeLength;
    public float chargeSpeed;
    public float chargeDamage;
    
    //Sound stuff;
    //Check if you can dash through if not fail;
    
    //Check when you reach target point or crash.
    //
    
    protected override void OnStart() 
    {
        
    }

    protected override void OnStop() 
    {
        
    }

    protected override State OnUpdate() 
    {
        return State.Success;
    }
}
