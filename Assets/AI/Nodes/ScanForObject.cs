using UnityEngine;
using TheKiwiCoder;

public class ScanForObject : ActionNode
{
    //public float maxWaitTime = 5f;
    public float scanRadius;
    public LayerMask layer;

    private Collider[] hits;
    
    protected override void OnStart()
    {
        hits = Physics.OverlapSphere(context.whaleBody.position, scanRadius, layer);
    }

    protected override void OnStop() 
    {
        
    }

    protected override State OnUpdate()
    {
        if (hits.Length <= 0) return State.Failure;
      
        
        
        return State.Success;
    }
}
