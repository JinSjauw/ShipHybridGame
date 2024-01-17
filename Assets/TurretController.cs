using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    [SerializeField] private Transform yAxle;
    [SerializeField] private Transform xAxle;

    #region Unity Functions

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion

    #region Private Functions

    #endregion

    #region Public Functions

    public void ShootHarpoon()
    {
        //Shoot...
        //Also apply force back onto the boat rigidBody;
        Debug.Log("Shot harpoon");
    }

    public void RotateHarpoon(float x, float y)
    {
        xAxle.rotation = Quaternion.AngleAxis(x, Vector3.up);
        yAxle.rotation = Quaternion.AngleAxis(y, Vector3.right);
    }

    #endregion
}
