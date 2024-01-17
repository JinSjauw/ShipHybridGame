using System;
using UnityEngine;
using System.IO.Ports;
using System.Threading;
using TMPro;

public class ArduinoReader : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    //[SerializeField] private BoatController boatController;
    [SerializeField] private BoatEngineSounds boatEngineSounds;


    private SerialPort stream = new SerialPort("COM3", 9600);

    private Thread thread;
    private bool isRunning = false;

    private string receivedString;
    private string[] data;
    private int turnAngle;
    private float thrust;

    private float maxTurnAngle = 25;
    private float maxThrustValue = 90;

    private int harpoonX;
    private int harpoonY;

    private int maxHarpoonXangle = 120;
    private int maxHarpoonYangle = 120;

    private bool harpoonButtonPressed;
    private int harpoonButton;
    private bool netButtonPressed;
    private int netButton;
    private bool craneButtonPressed;
    private int craneButton;

    #region Unity Functions

    private void Awake()
    {
        thread = new Thread (ThreadLoop);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isRunning) return;

        label.text = "Turn Angle: " + turnAngle + "\n"
                     + "Thrust: " + thrust * 100;
        boatEngineSounds.thrustBellSound(thrust);
        /*boatController.SetThrust(thrust);
        boatController.SetTurnAngle(turnAngle);*/
    }
    private void OnDisable()
    {
        stream.Close();
        thread.Abort();
    }

    #endregion

    private void ThreadLoop ()
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
                if (Mathf.Abs(turnAngle + turnRate) < maxTurnAngle)
                {
                    turnAngle += turnRate;
                }
            }

            if (float.TryParse(data[1], out float thrustValue))
            {
                if (thrustValue < 0)
                {
                    thrustValue = 0;
                }

                thrust = thrustValue / maxThrustValue;
            }

            if (Int32.TryParse(data[2], out int x))
            {
                //Max turnAngle is set here
                harpoonX = x;
            }

            if (Int32.TryParse(data[3], out int y))
            {
                //Max turnAngle is set here
                harpoonY = y;
            }

            if(Int32.TryParse(data[4], out int buttonState)) 
            {
                harpoonButton = buttonState;
            }
            if(Int32.TryParse(data[5], out int netButtonState)) 
            { 
                //Drop Net
                if (netButtonState == 1 && netButton != 1)
                {
                    netButtonPressed = true;
                }
                else
                {
                    netButton = 0;
                }
            }
            //Debug.Log(data.Length);
        }
    }

    public void Initialize(float turnAngleMax, float thrustValueMax)
    {
        maxTurnAngle = turnAngleMax;
        maxThrustValue = thrustValueMax;
    }

    public void StartThread()
    {
        // Creates and starts the thread
        thread.Start();
        isRunning = true;
    }

    public void StopThread()
    {
        thread.Abort();
        isRunning = false;
    }

    public bool IsRunning()
    {
        return isRunning;
    }

    public int GetTurnAngle()
    {
        return -turnAngle;
    }

    public float GetThrust()
    {
        return thrust;
    }

    public Vector3 GetHarpoonInput() 
    {
        return new Vector3(harpoonX, harpoonY, harpoonButton);
    }

    public int GetNetButton()
    {
        if (netButtonPressed)
        {
            netButtonPressed = false;
            return 1;
        }

        return 0;
    }
}
