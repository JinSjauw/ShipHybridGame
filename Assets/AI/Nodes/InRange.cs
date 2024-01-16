using UnityEngine;
using TheKiwiCoder;

public class InRange : ActionNode
{
    public float minRange;
    public float maxRange;
    public bool checkMaxRange;
    
    private Vector2 target;
    
    protected override void OnStart()
    {
        Vector3 playerTransform = blackboard.PlayerTransform.position;
        target = new Vector2(playerTransform.x, playerTransform.z);
    }

    protected override void OnStop() 
    {
        
    }

    protected override State OnUpdate()
    {
        Vector3 agentPosition = context.transform.position;
        float distance = Vector2.Distance(new Vector2(agentPosition.x, agentPosition.z), target);

        if (distance > minRange)
        {
            if (checkMaxRange)
            {
                if (distance < maxRange)
                {
                    return State.Success;
                }
                return State.Failure;
            }
            
            return State.Success;
        }
        return State.Failure;
    }
}
