using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressTurnOn : MonoBehaviour
{
    public GameObject Audio1;
    bool Klopt;
    // Start is called before the first frame update
    void Start()
    {
        Audio1.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.P))
        {
            Klopt = true;
        }
       

        if(Klopt == true)
        {
            Audio1.SetActive(true);
        }
        else
        {
            Audio1.SetActive(false);
        }
    }
}
