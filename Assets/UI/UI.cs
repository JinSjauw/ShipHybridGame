using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{



    [SerializeField] public float Throttle;
    [SerializeField] public int Health;
    [SerializeField] public float turnAngle;
    [SerializeField] public float FishAmount;

    public Transform wheel;

    public Slider Fishslider;
    public Slider Healthslider;
    public GameObject maskParent;

    public Sprite Left;
    public Sprite Right;

    public Sprite Neutral;

    [SerializeField]
    public Image uiImage;
    public Image turnsignal;

    [SerializeField]
    private Sprite[] imageArray;




    public void ChangeHealthImage()
    {

        Health = Mathf.Clamp(Health, 1, 5);


        if (uiImage != null && imageArray != null && imageArray.Length >= Health)
        {
            uiImage.sprite = imageArray[Health - 1];
        }

    }

    private void Start()
    {
        UpdateRectMask();
    }
    void Update()
    {
        Centerwheel();
        ChangeHealthImage();
        Turnsignal();
        ChangeFill();

    }
    private void Centerwheel()
    {

        float normalizedRotation1 = Mathf.InverseLerp(-40, 40, turnAngle);
        float remappedRotation1 = Mathf.Lerp(360, -360, normalizedRotation1);
        wheel.transform.rotation = Quaternion.Euler(0f, 0f, remappedRotation1);

    }
    private void Turnsignal()
    {
        if (turnAngle > 0)
        {
            turnsignal.sprite = Right;

        }
        if (turnAngle < 0)
        {
            turnsignal.sprite = Left;

        }
        if (turnAngle == 0)
        {
            turnsignal.sprite = Neutral;

        }
    }
    private void ChangeFill()
    {
        Fishslider.value = FishAmount;
        Healthslider.value = Health;
    }

    private void UpdateRectMask()
    {
        float normalizedthrottle = Mathf.InverseLerp(0, 1, Throttle);
        float remappedthrottle = Mathf.Lerp(81, 33, normalizedthrottle);

        float rectTop = remappedthrottle;


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
