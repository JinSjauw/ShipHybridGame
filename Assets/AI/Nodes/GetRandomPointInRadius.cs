using UnityEngine;
using TheKiwiCoder;

public class GetRandomPointInRadius : ActionNode
{
    public float radius;
    public bool playerCenter;

    private Vector2 center;
    
    protected override void OnStart()
    {
        center = playerCenter ? blackboard.PlayerTransform.position : context.whaleBody.position;
    }

    protected override void OnStop() 
    {
        
    }

    protected override State OnUpdate()
    {
        Vector2 targetPosition = center + Random.insideUnitCircle * radius;
        blackboard.moveToPosition = targetPosition;
        
        return State.Success;
    }
}
