using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatEngineSounds : MonoBehaviour
{

    [SerializeField] private AudioController audioController;

    public float minSpeed;
    public float maxSpeed;
    private float currentSpeed;

    private Rigidbody boatRB;
    private AudioSource boatAudio;

    public float minPitch;
    public float maxPitch;
    private float pitchFromBoat;
    public bool isEngine;

    private void Start()
    {
        boatAudio = GetComponent<AudioSource>();
        boatRB = GetComponentInParent<Rigidbody>();

    }

    private void Update()
    {
       if(isEngine == true)
        {
            EngineSound();
        }
       
    }


     void EngineSound() // minSpeed = 0.3, maxSpeed = 35, minPitch -0.1 and maxPitch 0.2
    {
        currentSpeed = boatRB.velocity.magnitude;
       // Debug.Log("speed: " + currentSpeed);
        pitchFromBoat = boatRB.velocity.magnitude / 80f;

        if (currentSpeed < minSpeed)
        {
            boatAudio.pitch = minPitch;
        }

        if(currentSpeed > minSpeed && currentSpeed < maxSpeed)
        {
            boatAudio.pitch = minPitch + pitchFromBoat;
        }

        if (currentSpeed > maxSpeed)
        {
            boatAudio.pitch = maxPitch;
        }

    }

    public void thrustBellSound(int thrustValue) //takes the thrustvalue from the Arduino and calls the bell when the thrust gets past 0 and 50
    {
        Debug.Log("thrust:  " + thrustValue);

        if (thrustValue < 50 && thrustValue > 0) 
        {
            audioController.PlayAudio(Audio.AudioID.BOATSOUNDS);
        } else if (thrustValue > 50 && thrustValue < 100)
        {
            audioController.PlayAudio(Audio.AudioID.BOATSOUNDS);
        }
    }
}
