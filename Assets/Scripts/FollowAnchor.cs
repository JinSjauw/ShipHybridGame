using UnityEngine;

public class FollowAnchor : MonoBehaviour
{
    [SerializeField] private Transform anchorBody;
    [SerializeField] private Rigidbody ballBody;
    
    [SerializeField] private float yOffset;
    [SerializeField] private float maxDistance;
    [SerializeField] private float maxFollowPower;
    [SerializeField] private AnimationCurve followPowerPercentage;
    
    [SerializeField] private bool debug;
    
    private Vector3 targetPosition;
    private Vector3 targetDirection;
    
    // Start is called before the first frame update
    void Start()
    {
        ballBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        targetPosition = anchorBody.position;
        targetPosition.y += yOffset;
        
        Follow();
        
        /*if (Vector3.Distance(targetPosition, transform.position) > maxDistance)
        {
            Follow();
        }*/
    }

    private void Follow()
    {
        targetDirection = targetPosition - transform.position;

        float distance = Vector3.Distance(targetPosition, transform.position);
        float offset = Mathf.Clamp01(distance / maxDistance);
        
        float force = ballBody.mass * (followPowerPercentage.Evaluate(offset) * maxFollowPower);
        
        Log("Offset: " + offset + " Force: " + force);
        
        ballBody.AddForce(targetDirection * force);
        //ballBody.MovePosition(targetPosition);
    }

    private void Log(string _msg)
    {
        if (!debug) return;
        
        Debug.Log("[FollowAnchor]: " + _msg);
    }
}
