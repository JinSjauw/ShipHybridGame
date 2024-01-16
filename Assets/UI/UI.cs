using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField]
    private Image uiImage;

    [SerializeField]
    private Sprite[] imageArray;

    [SerializeField, Range(1, 5)]
    private int number = 1;
    public GameObject maskParent;
    public Slider slider;
    public Image Image;

    [SerializeField] public float Throttle;
    [SerializeField] public int Health;
    [SerializeField] public float turnAngle;
    [SerializeField] public float FishAmount;

    // Function to change the UI image based on the input number
    public void ChangeImageBasedOnNumber()
    {
        // Ensure the number is within the valid range (1 to 5)
        number = Mathf.Clamp(number, 1, 5);

        // Set the UI image sprite based on the number
        if (uiImage != null && imageArray != null && imageArray.Length >= number)
        {
            uiImage.sprite = imageArray[number - 1];
        }
        else
        {
            Debug.LogError("Invalid setup: Please assign UI Image and ensure the image array is set up correctly.");
        }
    }

    private void Start()
    {
        UpdateRectMask();
    }

    private void UpdateRectMask()
    {
        // Update the padding based on the slider value
        // For example, if the slider controls the top position of the rectangle:
        float rectTop = Throttle;


        // Update the RectMask2D's padding
        RectMask2D rectMask = maskParent.GetComponent<RectMask2D>();
        if (rectMask != null)
        {
            Vector4 currentPadding = rectMask.padding;
            rectMask.padding = new Vector4(currentPadding.x, 0, currentPadding.z, +rectTop);
        }
    }
    // Called in the editor when a value is changed in the Inspector
    private void OnValidate()
    {
        UpdateRectMask();
    }
}
