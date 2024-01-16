using TheKiwiCoder;
using UnityEngine;

public class ChargeAttack : ActionNode
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
    
    //Sound stuff;
    //Check if you can dash through if not fail;
    
    //Check when you reach target point or crash.
    //
    
    protected override void OnStart()
    {
        Debug.Log("Charge Attack!");
        
        //targetDirection = blackboard.PlayerTransform.position - context.transform.position;
        //targetPosition = targetDirection.normalized * chargeLength;

        originPosition = context.whaleBody.position;
        targetPosition = blackboard.PlayerTransform.position;
        
        origin2D = new Vector2(originPosition.x, originPosition.z);
        target2D = new Vector2(targetPosition.x, targetPosition.z);
        
        totalDistance = Vector2.Distance(origin2D, target2D);
        context.agent.stoppingDistance = stoppingDistance;
        context.agent.speed = chargeSpeed;
        
        context.agent.destination = targetPosition;
        context.agent.autoBraking = false;
        context.agent.updateRotation = true;
        context.agent.acceleration = acceleration;
    }

    protected override void OnStop() 
    {
        
    }

    protected override State OnUpdate() 
    {
        //Do the charge attack
        //Player detects Collision;
        Vector3 currentPosition = context.transform.position;
        distance = Vector2.Distance(new Vector2(currentPosition.x, currentPosition.z), target2D);

        normalizedDistance = 1 - Mathf.Clamp01(distance / totalDistance);
        
        Debug.Log("[ChargeAttack]:[distance]: " + distance + " [maxDistance]: " + totalDistance);
        Debug.Log("[ChargeAttack]:[NormalizedDistance]: " + normalizedDistance);

        float depth = Mathf.Lerp(originPosition.y, WaveManager.Instance.FindPoint(target2D).y, depthCurve.Evaluate(normalizedDistance));

        context.whaleBody.position = new Vector3(currentPosition.x, depth + targetDepthOffset, currentPosition.z);
        
        /*context.whaleBody.position = new Vector3(currentPosition.x,
            depthCurve.Evaluate(normalizedDistance) * targetPosition.y, currentPosition.z);*/
        
        if (context.agent.pathPending)
        {
            return State.Running;
        }

        if (context.agent.remainingDistance < tolerance) 
        {
            //context.whaleBody.position =  new Vector3(currentPosition.x, originPosition.y, currentPosition.z);
            return State.Success;
        }

        if (context.agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid) 
        {
            return State.Failure;
        }

        return State.Running;
    }
}
