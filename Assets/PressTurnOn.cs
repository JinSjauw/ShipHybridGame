using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressTurnOn : MonoBehaviour
{
    public BoatController boatController;
    public bool Klopt;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
       
   
    }

    public void DriftCamera()
    {
        var isDrifting = boatController.IsDrifting();

        if (isDrifting)
        {
            Klopt = true;
            Debug.Log("het werkt!!!");
       
       }
        else
        {

        }

    }
         
}
