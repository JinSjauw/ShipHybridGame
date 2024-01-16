using TheKiwiCoder;
using UnityEngine;

public class MoveToPosition : ActionNode
{
    public float speed = 5;
    public float stoppingDistance = 0.1f;
    public bool updateRotation = true;
    public float acceleration = 40.0f;
    public float tolerance = 1.0f;

    public AnimationCurve diveCurve;
    public AnimationCurve depthCurve;
    public float depthOffset;
    public float minDepth;
    
    private Vector3 originPosition;
    private Vector3 targetPosition;
    private Vector3 currentPosition;
    private Vector2 origin2D;
    private Vector2 target2D;
   
    private float distance;
    private float totalDistance;
    private float normalizedDistance;
    private float current;
    
    protected override void OnStart()
    {
        originPosition = context.whaleBody.position;
        targetPosition = blackboard.moveToPosition;
        
        origin2D = new Vector2(originPosition.x, originPosition.z);
        target2D = new Vector2(targetPosition.x, targetPosition.z);
        
        totalDistance = Vector2.Distance(origin2D, target2D);
        
        context.agent.stoppingDistance = stoppingDistance;
        context.agent.speed = speed;
        
        context.agent.destination = targetPosition;
        context.agent.autoBraking = false;
        context.agent.updateRotation = updateRotation;
        context.agent.acceleration = acceleration;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate()
    {
        currentPosition = context.whaleBody.position;
        distance = Vector2.Distance(new Vector2(currentPosition.x, currentPosition.z), target2D);
        normalizedDistance = 1 - Mathf.Clamp01(distance / totalDistance);
        
        current = Mathf.MoveTowards(current, 1, speed * Time.deltaTime);

        if (current >= 1) current = 0;
        
        //Move whale to acceptable depth range
        if (currentPosition.y > minDepth)
        {
            float depth = Mathf.Lerp(originPosition.y, minDepth, diveCurve.Evaluate(normalizedDistance));
            context.whaleBody.position = new Vector3(currentPosition.x, depth, currentPosition.z);
        }
        else
        {
            float depth = WaveManager.Instance.FindPoint(target2D).y - depthOffset;
            context.whaleBody.position = new Vector3(currentPosition.x, depth, currentPosition.z);
        }
        
        if (context.agent.pathPending) {
            return State.Running;
        }

        if (context.agent.remainingDistance < tolerance) {
            return State.Success;
        }

        if (context.agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid) {
            return State.Failure;
        }

        return State.Running;
    }
}
