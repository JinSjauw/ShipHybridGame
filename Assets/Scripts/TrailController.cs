using UnityEngine;

public class TrailController : MonoBehaviour
{
    public Rigidbody rb;
    public TrailRenderer trailRenderer;
    public float velocityThreshold = 5f; // Adjust this value as needed

    void Update()
    {
        // Check if the Rigidbody's velocity magnitude is greater than the threshold
        if (rb.velocity.magnitude > velocityThreshold)
        {
            // Turn on the trail renderer
            if (!trailRenderer.enabled)
            {
                trailRenderer.enabled = true;
            }
        }
        else
        {
            // Turn off the trail renderer
            if (trailRenderer.enabled)
            {
                trailRenderer.enabled = false;
            }
        }
    }
}
