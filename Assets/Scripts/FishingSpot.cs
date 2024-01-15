using System;
using UnityEngine;

public class FishingSpot : MonoBehaviour
{
    [SerializeField] private bool driftFishing;

    [Header("Static Fishing")]
    [SerializeField] private float fishingRate;


    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        
    }
}
