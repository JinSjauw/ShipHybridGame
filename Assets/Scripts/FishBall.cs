using UnityEngine;

public class FishBall : MonoBehaviour
{
    [SerializeField] private Transform fishes;
    private float fillPercentage;
    
    public void SetFillPercentage(float fill)
    {
        fillPercentage = fill;
    }

    public void SetFishContent(Vector3 scale)
    {
        fishes.localScale = scale;
    }

    public float GetFillPercentage()
    {
        return fillPercentage;
    }
}
