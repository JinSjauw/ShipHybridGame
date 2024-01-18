using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField] private bool FollowWaves;
    [SerializeField] private Transform fishBody;
    [SerializeField] private float depthOffset;
    
    void FixedUpdate()
    {
        /*if (FollowWaves)
        {
            Vector3 newPosition = fishBody.transform.position;
            newPosition.y = WaveManager.Instance.FindPoint(new Vector2(newPosition.x, newPosition.z)).y - depthOffset;
            fishBody.transform.position = newPosition;
        }    */
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
