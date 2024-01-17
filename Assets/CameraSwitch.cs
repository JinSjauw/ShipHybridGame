using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public BoatController boatController;

    public GameObject MainCamera;
    public GameObject DriftLeft;
    public GameObject DriftRight;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DriftCamera();
    }

    public void DriftCamera()
    {
        var SidewaysVelocity = boatController.SidewaysVelocity();
        var isDrifting = boatController.IsDrifting();

        if(isDrifting)
        {
            Debug.Log("isdrifting");
            MainCamera.SetActive(false);

            if(SidewaysVelocity < 0)
            {
                Debug.Log("Links");
                DriftLeft.SetActive(true);
                DriftRight.SetActive(false);
            }

            if(SidewaysVelocity > 0)
            {
                Debug.Log("Rechts");
                DriftLeft.SetActive(false);
                DriftRight.SetActive(true);
            }
        }
        else
        {
            MainCamera.SetActive(true);
            DriftLeft.SetActive(false);
            DriftRight.SetActive(false);
        }
    }
}
