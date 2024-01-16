using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{

    public GameObject AllFish;
    // Start is called before the first frame update
    void Start()
    {

        AllFish = GameObject.FindWithTag("AllFish");
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
            AllFish.GetComponent<FishSound>().Score ++;
            AllFish.GetComponent<FishSound>().Fishcount++;
            Destroy(gameObject);


        }
    }
}
