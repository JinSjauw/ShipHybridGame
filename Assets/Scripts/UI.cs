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
    void Update()
    {
        Debug.Log("Update is called!");
        wheel.transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);
    }
}
