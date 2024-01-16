using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class ScanForObject : ActionNode
{
    //public float maxWaitTime = 5f;
    public float scanRadius;
    public LayerMask layer;
    
    protected override void OnStart()
    {
        //Physics.SphereCastAll()
    }

    protected override void OnStop() 
    {
        
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}
