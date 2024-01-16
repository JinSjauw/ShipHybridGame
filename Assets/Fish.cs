using UnityEngine;

public class Fish : MonoBehaviour
{
    //public GameObject AllFish;
    // Start is called before the first frame update
    void Start()
    {

        //AllFish = GameObject.FindWithTag("AllFish");
     // Fishcount.GetComponent<FishSound>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("[Fish]: Hit " + other.name + "!");
        if(other.TryGetComponent(out BoatController boatController))
        {
            //Debug.Log("[Fish]: Hit with BoatController!");
            NetController netController = boatController.GetNetController();
            if (netController.IsHookLowered() && boatController.GetNetController().AddFish())
            {
                //Debug.Log("[Fish]: Fishing!");
                Destroy(gameObject);   
            }
            /*AllFish.GetComponent<FishSound>().Score ++;
            AllFish.GetComponent<FishSound>().Fishcount++;
            Destroy(gameObject);*/
        }
    }
}
