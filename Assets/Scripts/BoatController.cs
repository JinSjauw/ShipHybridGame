using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class BoatController : MonoBehaviour, IDamageable
{
    [Header("Boat Physics References")]

    [SerializeField] private Transform engineTransform;
    [SerializeField] private List<Transform> accelerationPoints;
    [SerializeField] private List<Floater> floaters;
    [SerializeField] private Transform nose;
    [SerializeField] private bool showForces;

    [Header("Boat Locomotion")]

    [Header("Arduino Parameters")]
    [Header("Use Arduino")][SerializeField] private bool useArduino;
    [SerializeField] private float maxArduinoTurnAngle;
    [SerializeField] private float maxArduinoThrustValue;
    private ArduinoReader arduinoReader;

    [Header("Boat Acceleration")]
    [SerializeField] private float boatTopSpeed;
    [SerializeField] private AnimationCurve powerCurve;

    [Header("Boat Steering")]
    [SerializeField] private float turnSpeed;
    [SerializeField] private float maxTurnAngle = 40;
    [SerializeField] private AnimationCurve dragCurve;
    [SerializeField] private AnimationCurve frictionCurve;

    [Header("Boat Buoyancy")]
    [SerializeField] private float springStrength;
    [SerializeField] private float damperStrength;

    [Header("Boat VFX")]

    [SerializeField] private ParticleSystem leftDriftParticle;
    [SerializeField] private ParticleSystem rightDriftParticle;
    [SerializeField] private ParticleSystem wakeParticle;
    [SerializeField] private float minStartDriftVelocity;
    [SerializeField] private float minEndDriftVelocity;

    [Header("Auxiliary Controllers")]
    [SerializeField] private NetController netController;
    [SerializeField] private TurretController turretController;

    private Rigidbody boatBody;

    private bool isMoving;
    private bool isTurning;
    private bool isDrifting;

    [SerializeField] private float throttle;
    [SerializeField] private float thrust;
    [SerializeField] private float boatSpeed;
    private float normalizedSpeed;

    private float turnRate;
    [SerializeField] public float turnAngle;

    private int Health { get; set; }

    private bool debug;

    #region Unity Functions

    private void Awake()
    {
        arduinoReader = GetComponent<ArduinoReader>();
        boatBody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        foreach (Floater floater in floaters)
        {
            floater.SetDragCurve(dragCurve);
        }

        foreach (Transform accelPoint in accelerationPoints)
        {
            if (accelPoint.TryGetComponent(out Floater floater))
            {
                floater.SetFrictionCurve(frictionCurve);
            }
        }

        //Initialize ArduinoReader;
        arduinoReader.Initialize(maxArduinoTurnAngle, maxArduinoThrustValue);
        if (useArduino && !arduinoReader.IsRunning())
        {
            arduinoReader.StartThread();
        }
    }

    void FixedUpdate()
    {
        if (!useArduino && arduinoReader.IsRunning())
        {
            arduinoReader.StopThread();
        }
        else if (useArduino && !arduinoReader.IsRunning())
        {
            arduinoReader.StartThread();
        }

        KeepUpright();
        HandleInput();
        DisplayParticles();

        if (showForces)
        {
            DrawArrow.ForDebug(nose.transform.position, boatBody.velocity.normalized * 3f * normalizedSpeed, Color.blue);
        }
    }
    #endregion

    #region Unity Input Callbacks

    public void OnThrust(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            throttle = context.ReadValue<float>();
        }

        if (context.canceled)
        {
            throttle = 0;
        }
    }
    public void OnSteer(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            turnRate = context.ReadValue<float>();
            if (turnRate != 0)
            {
                isTurning = true;

                if (isDrifting)
                {
                    turnAngle = 0;
                }
            }
        }

        if (context.canceled)
        {
            turnRate = 0;
            turnAngle = 0;
            isTurning = false;
        }
    }

    public void OnFishing(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            netController.ActuateHook();
        }
    }

    #endregion

    #region Private Functions

    private void HandleInput()
    {
        boatSpeed = Vector3.Dot(transform.forward, boatBody.velocity);
        normalizedSpeed = Mathf.Clamp01(Mathf.Abs(boatSpeed / boatTopSpeed));

        float thrustValue = useArduino ? arduinoReader.GetThrust() : throttle;

        if (thrustValue > 0 && WaterCheck())
        {
            if (normalizedSpeed >= .99f)
            {
                return;
            }

            float availablePower = powerCurve.Evaluate(normalizedSpeed) * thrustValue * boatBody.mass;

            foreach (Transform point in accelerationPoints)
            {
                Vector3 accelDirection = point.forward;
                accelDirection.y = 0;
                Vector3 forwardForce = accelDirection * availablePower * 10f;
                boatBody.AddForceAtPosition(forwardForce, point.position);
                DrawArrow.ForDebug(point.position, accelDirection * availablePower, Color.yellow);
            }
        }

        if (isTurning && !useArduino)
        {
            //Applies torque;
            if (Mathf.Abs(turnAngle + turnRate) < maxTurnAngle)
            {
                float angleChange = turnRate * turnSpeed * Time.fixedDeltaTime;
                turnAngle += angleChange;
                turnAngle = Mathf.Clamp(turnAngle, -maxTurnAngle, maxTurnAngle);

                Vector3 angularVelocity = new Vector3(0, turnAngle, 0);
                //Quaternion deltaRotation = Quaternion.Euler(turnSpeed * angularVelocity * Time.fixedDeltaTime);

                boatBody.angularVelocity = (turnSpeed * angularVelocity) * normalizedSpeed;
            }
        }
        else
        {
            Vector3 angularVelocity = new Vector3(0, arduinoReader.GetTurnAngle(), 0);
            boatBody.angularVelocity = (turnSpeed * angularVelocity) * normalizedSpeed;
        }
    }
    private void KeepUpright()
    {
        float offset = Mathf.Clamp01(1f - Vector3.Dot(Vector3.up, boatBody.transform.up));

        Vector3 springTorque = boatBody.mass * (offset * springStrength) * Vector3.Cross(boatBody.transform.up, Vector3.up);
        Vector3 dampTorque = boatBody.mass * (damperStrength) * boatBody.angularVelocity;

        boatBody.AddTorque(springTorque - dampTorque);
    }
    private bool WaterCheck()
    {
        Vector3 enginePosition = engineTransform.position;
        if (WaveManager.Instance.FindPoint(new Vector2(enginePosition.x, enginePosition.z)).y > enginePosition.y)
        {
            return true;
        }
        return false;
    }
    private void DisplayParticles()
    {
        float velocity = SidewaysVelocity();
        float normalizedVelocity = Mathf.InverseLerp(7, 30, Mathf.Abs(velocity));

        // Map the normalized value to the range [0.20, 1]
        float remappedVelocity = Mathf.Lerp(0.1f, 1f, normalizedVelocity);

        float thrustValue = useArduino ? thrust : throttle;

        if (thrustValue > 0 && WaterCheck())
        {
            if (!wakeParticle.isPlaying)
            {
                wakeParticle.Play();
            }

            if (Mathf.Abs(velocity) > minStartDriftVelocity)
            {
                switch (velocity)
                {
                    case > 0:
                        rightDriftParticle.Play();
                        rightDriftParticle.startSize = remappedVelocity;
                        break;
                    case < 0:
                        leftDriftParticle.Play();
                        leftDriftParticle.startSize = remappedVelocity;
                        break;
                }
                isDrifting = true;
            }
        }
        else
        {
            wakeParticle.Stop();
        }
        if (Mathf.Abs(velocity) <= minEndDriftVelocity)
        {
            if (rightDriftParticle.isPlaying)
            {
                rightDriftParticle.Stop();
            }
            if (leftDriftParticle.isPlaying)
            {
                leftDriftParticle.Stop();
            }
            isDrifting = false;
        }
    }

    private void Log(string _msg)
    {
        if (!debug) return;
        Debug.Log("[BoatController]: " + _msg);
    }

    #endregion

    #region Public Functions

    private float SidewaysVelocity()
    {
        return Vector3.Dot(boatBody.velocity, transform.right);
    }

    public NetController GetNetController()
    {
        return netController;
    }

    #endregion

    #region IDamageable Functions

    public void TakeDamage(int damage)
    {
        Health -= damage;
    }

    public void SetHealth(int amount)
    {
        Health = amount;
    }

    public void Die()
    {
        Log("Died!");
    }

    #endregion


}
