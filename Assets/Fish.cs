using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{

    

    public GameObject Fishcount;
    // Start is called before the first frame update
    void Start()
    {

        Fishcount = GameObject.FindWithTag("AllFish");
     // Fishcount.GetComponent<FishSound>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "BOAT")
        {

            Fishcount.GetComponent<FishSound>().Fishcount ++;
            Destroy(gameObject);


        }
    }
}
