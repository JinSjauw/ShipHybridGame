using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingSpot : MonoBehaviour
{
    //[SerializeField] private bool driftFishing;

    [Header("Static Fishing")] 
    [SerializeField] private float fishingTime;
    [SerializeField] private int fishingRate;

    private NetController netController;
    private float timer;
    
    private void OnTriggerEnter(Collider other)
    {
        if (netController != null) return;
        
        if(other.TryGetComponent(out BoatController boatController))
        {
            Debug.Log("[Fish]: Hit with BoatController!");
            netController = boatController.GetNetController();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (netController.IsHookLowered())
        {
            if (timer < fishingTime)
            {
                timer += Time.deltaTime;
            }
            else
            {
                Debug.Log("[Fishing Spot]: Added " + fishingRate + " Fish!");
                netController.AddFish(fishingRate);
                timer = 0;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exiting Fishing Spot! " + other.name);
        netController = null;
    }
}
