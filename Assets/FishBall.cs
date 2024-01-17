using UnityEngine;

public class FishBall : MonoBehaviour
{
    private float fillPercentage;

    public void SetFillPercentage(float fill)
    {
        fillPercentage = fill;
    }

    public float GetFillPercentage()
    {
        return fillPercentage;
    }
}
