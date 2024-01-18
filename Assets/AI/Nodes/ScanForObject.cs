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
        
        //Get closest Object
        float distance;
        float closestDistance = scanRadius + 1;
        Transform closestTransform = null;
        foreach (Collider col in hits)
        {
            Vector3 colPosition = col.transform.position;
            distance = Vector3.Distance(colPosition, context.whaleBody.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTransform = col.transform;
            }
        }

        blackboard.targetFishBall = closestTransform;

        return State.Success;
    }
}
