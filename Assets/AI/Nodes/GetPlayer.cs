using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class GetPlayer : ActionNode
{
    protected override void OnStart() 
    {
        
    }

    protected override void OnStop() 
    {
        
    }

    protected override State OnUpdate()
    {
        Transform target = FindObjectOfType<BoatController>().transform;

        if (target == null)
        {
            return State.Failure;
        }

        blackboard.PlayerTransform = target;
        
        return State.Success;
    }
}
