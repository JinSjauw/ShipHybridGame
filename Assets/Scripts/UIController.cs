using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Transform wheel;
    [SerializeField] private Slider fishBar;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider throttleBar;

    [SerializeField] private Image turnSignal;
    [SerializeField] private Sprite[] turnSignals;

    [SerializeField] private Image healthState;
    [SerializeField] private Sprite[] healthImageArray;
    
    private BoatController boatController;

    private float turnAngle;
    
    private void Awake()
    {
        boatController = FindObjectOfType<BoatController>();
        if (boatController != null)
        {
            boatController.SetUIController(this);
        }
    }

    private void Update()
    {
        //if (boatController == null) return;
        
        UpdateTurning();
    }

    private void UpdateTurning()
    {
        Vector2 turnValues = boatController.GetTurnValues();
        turnAngle = turnValues.x;
        
        //float turnPercentage = Mathf.Clamp01(turnAngle / turnValues.y);

        float normalizedRotation1 = Mathf.InverseLerp(-40, 40, turnAngle);
        float remappedRotation1 = Mathf.Lerp(360, -360, normalizedRotation1);
        wheel.transform.rotation = Quaternion.Euler(0f, 0f, remappedRotation1);

        //wheel.transform.rotation = Quaternion.AngleAxis(turnPercentage * turnAngle * 100, Vector3.forward);
    }

    public void UpdateThrottle(float percentage)
    {
        throttleBar.value = percentage;
    }
    
    public void UpdateFishing(float percentage)
    {
        fishBar.value = percentage;
    }

    public void UpdateHealth(float percentage)
    {

        healthBar.value = percentage;
        
        Debug.Log("Health %: " + percentage);
        //Updating Health State Image
        if (healthState != null && healthImageArray != null)
        {
            switch (percentage)
            {
                case >= .8f:
                    healthState.sprite = healthImageArray[4];
                    break;
                case >= .6f:
                    healthState.sprite = healthImageArray[3];
                    break;
                case >= .4f:
                    healthState.sprite = healthImageArray[2];
                    break;
                case >= .2f:
                    healthState.sprite = healthImageArray[1];
                    break;
                case >= 0:
                    healthState.sprite = healthImageArray[0];
                    break;
            }
        }
    }
}
