using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBoat : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private float zOffset;
    [SerializeField] private float yOffset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = new Vector3(target.position.x, yOffset, target.position.z);
        transform.position = newPosition;
    }
}
