using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harpoon : MonoBehaviour
{

    public Transform BulletSpawnPoint;
    public GameObject BulletPrefab;
    public float bulletspeed;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Rigidbody bulletBody = Instantiate(BulletPrefab, BulletSpawnPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
            bulletBody.transform.forward = BulletSpawnPoint.forward;
            bulletBody.AddForce(bulletBody.mass * (BulletSpawnPoint.forward * bulletspeed));
        }
    }
    // Start is called before the first frame update

}
