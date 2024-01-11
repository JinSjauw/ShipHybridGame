using UnityEngine;
using TheKiwiCoder;

public class GetRandomPointInRadius : ActionNode
{
    public float radius;
    
    private Vector2 center;
    
    protected override void OnStart()
    {
        center = blackboard.PlayerTransform.position;
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
