using System;
using UnityEngine;

public class NetController : MonoBehaviour
{
    [SerializeField] private Rigidbody netBody;
    [SerializeField] private Transform anchor;
    
    // Have the ball follow an anchor;

    private void FixedUpdate()
    {
        netBody.MovePosition(anchor.position);
    }
}
