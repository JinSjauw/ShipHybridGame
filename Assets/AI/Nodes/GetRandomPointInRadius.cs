using UnityEngine;
using TheKiwiCoder;

public class GetRandomPointInRadius : ActionNode
{
    public float radius;
    public bool playerCenter;

    private Vector3 center;
    
    protected override void OnStart()
    {
        center = playerCenter ? blackboard.PlayerTransform.position : context.whaleBody.position;
    }

    protected override void OnStop() 
    {
        
    }

    protected override State OnUpdate()
    {
        Vector2 targetPosition = new Vector2(center.x, center.z) + Random.insideUnitCircle * radius;
        blackboard.moveToPosition = new Vector3(targetPosition.x, 0, targetPosition.y);
        
        return State.Success;
    }
}
