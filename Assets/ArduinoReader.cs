using System;
using System.Collections;
using UnityEngine;
using System.IO.Ports;
using System.Threading;
using TMPro;


public class ArduinoReader : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private BoatController boatController;
    [SerializeField] private BoatEngineSounds boatEngineSounds;


    private SerialPort stream = new SerialPort("COM3", 9600);
    
    private Thread thread;

    private string receivedString;
    private string[] data;
    private int turnAngle;
    private int thrust;
    
    // Start is called before the first frame update
    void Start()
    {
        //stream.Open();
        StartThread();
    }

    // Update is called once per frame
    private void Update()
    {
        label.text = "Turn Angle: " + turnAngle + "\n" 
                     + "Thrust: " + thrust;

      
        boatController.SetThrust(thrust);
        boatController.SetTurnAngle(turnAngle);
        boatEngineSounds.thrustBellSound(thrust);
    }

    public void StartThread ()
    {
        // Creates and starts the thread
        thread = new Thread (ThreadLoop);
        thread.Start();
    }
    
    public void ThreadLoop ()
    {
        stream.Open();

        while (true)
        {
            receivedString = stream.ReadLine();
            stream.BaseStream.Flush();
        
            if(receivedString == null) return;
        
            data = receivedString.Split(",");
            
            if (Int32.TryParse(data[0], out int turnRate))
            {
                //Max turnAngle is set here
                if (Mathf.Abs(turnAngle + turnRate) < 25)
                {
                    turnAngle += turnRate;
                }
            }
            
            if (Int32.TryParse(data[1], out int thrustPower))
            {
                thrust = thrustPower;
            }
        
            //Debug.Log(data.Length);
        }
    }
    
    private void OnDisable()
    {
        stream.Close();
    }
}
