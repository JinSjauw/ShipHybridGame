using UnityEngine;
using TheKiwiCoder;

public class ArtilleryAttack : ActionNode
{
    public AnimationCurve depthCurve;
    public float speed;
    public float targetDepthOffset = 2f;

    public int launchAmount;

    private Vector3 originPosition;
    private Vector2 origin2D;

    private float current;
    private float desiredDepth;
    
    protected override void OnStart() 
    {
        Debug.Log("Artillery Attack!");
        
        originPosition = context.whaleBody.position;
        origin2D = new Vector2(originPosition.x, originPosition.z);

        current = 0;
    }

    protected override void OnStop() 
    {
        
    }

    protected override State OnUpdate()
    {
        Vector3 currentPosition = context.whaleBody.position;
        current = Mathf.MoveTowards(current, 1, speed * Time.deltaTime);
        desiredDepth = WaveManager.Instance.FindPoint(origin2D).y;
        float depth = Mathf.Lerp(originPosition.y, desiredDepth, depthCurve.Evaluate(current));
        
        context.whaleBody.position = new Vector3(currentPosition.x, depth - targetDepthOffset, currentPosition.z);

        if (current >= .8f)
        {
            context.animator.Play("Sneeze");
        }
        
        if (current >= 1)
        {
            //Do the shooting;
            context.whaleLauncher.Launch(launchAmount);
            return State.Success;
        }

        return State.Running;
    }
}
