using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public Transform wheel;
    public float rotationAngle;

    void Start()
    {
        // You can add initialization code here if needed
    }

    // Update is called once per frame
    public float rotationSpeed = 45f;

    void Update()
    {
        // Rotate the object continuously around the Y-axis
        wheel.transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}
