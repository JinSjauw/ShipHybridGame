using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriftSound : MonoBehaviour
{
    
    public AudioSource myAudioSource;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

        myAudioSource.Play();
        if (Input.GetKeyDown(KeyCode.U))
        {
            myAudioSource.Stop();
        }

    }
}
