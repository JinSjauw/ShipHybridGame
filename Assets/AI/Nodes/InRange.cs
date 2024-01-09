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
        target = blackboard.PlayerTransform.position;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate()
    {
        float distance = Vector2.Distance(blackboard.moveToPosition, target);

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
