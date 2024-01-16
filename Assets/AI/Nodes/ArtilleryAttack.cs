using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class ArtilleryAttack : ActionNode
{
    protected override void OnStart() 
    {
        Debug.Log("Artillery Attack!");
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        
        return State.Success;
    }
}
