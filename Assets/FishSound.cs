using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSound : MonoBehaviour
{

     public int Fishcount;
     public int Score;
    public int FishCountMax;
     public bool fishNetIsFull;
    public AudioClip FishNetFull;
     public AudioClip FishCatch1;
     public AudioClip FishCatch2;
     public AudioClip FishCatch3;
     public AudioClip FishCatch4;
     public AudioClip FishCatch5;
     AudioSource AS;
    // Start is called before the first frame update
    void Start()
    {
        AS = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {


        if (Fishcount > FishCountMax)
        {
            fishNetIsFull = true;
        }


        if(Score == 3)
        {
            AS.PlayOneShot(FishCatch1,1);
            Fishcount++;
        }

        if (Score == 7)
        {
            AS.PlayOneShot(FishCatch2);
            Fishcount++;
        }

        if (Score == 10)
        {
            AS.PlayOneShot(FishCatch3);
            Fishcount++;
        }
        
        if (Score == 13)
        {
            AS.PlayOneShot(FishCatch4);
            Fishcount++;
        }

        if (Score > 17)
        {
            AS.PlayOneShot(FishCatch5);
            Fishcount = 1;
            Fishcount++;
        }
    }
}
