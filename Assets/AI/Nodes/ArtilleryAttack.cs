using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class ArtilleryAttack : ActionNode
{
    
    public AnimationCurve depthCurve;
    public float chargeSpeed;
    public float tolerance = 1f;
    public float stoppingDistance = 0.1f;
    public float acceleration = 40.0f;
    public float targetDepthOffset = 2f;
    
    private Vector3 originPosition;
    private Vector3 targetPosition;
    private Vector2 origin2D;
    private Vector2 target2D;
    
    private float distance;
    private float totalDistance;
    private float normalizedDistance;
    
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
