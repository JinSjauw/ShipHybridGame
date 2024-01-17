using System;
using System.Collections;
using Audio;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class NetController : MonoBehaviour
{
    [Header("Hook & Crane")]
    [SerializeField] private Transform anchor;
    [SerializeField] private Transform hook;
    
    [SerializeField] private AnimationCurve hookRotationCurve;
    [SerializeField] private float actionSpeed = 3;
    
    [SerializeField] private float loweredRotation;
    [SerializeField] private float neutralRotation;

    [Header("Net")] 
    [SerializeField] private RopeController ropeController;
    [SerializeField] private Transform fishBallPrefab;
    [SerializeField] private Transform fishnetTransform;
    [SerializeField] private Transform netTransform;
    [SerializeField] private Transform netBody;
    [SerializeField] private Transform fishBall;
    [SerializeField] private float minNetRadius = 0.2f;
    [SerializeField] private float maxNetRadius = 0.5f;
    [SerializeField] private float minFishBallRadius = 0.0f;
    [SerializeField] private float maxFishBallRadius = 1.2f;

    [Header("Fishing")]
    [SerializeField] private int maxFishCount = 100;

    [Header("Fish Audio")] 
    [SerializeField] private AudioID fishNetFull;


    [SerializeField] private int fishAudioStep = 4;
    [SerializeField] private int fishAudioCount;
    public AudioClip[] Sounds;
    public AudioClip clip;
    public AudioSource myAudioSource;


    [SerializeField] private AudioID fishCatch1;
    [SerializeField] [Range(0, .2f)]private float playThreshold1;
    [SerializeField] private AudioID fishCatch2;
    [SerializeField] [Range(0, .4f)]private float playThreshold2;
    [SerializeField] private AudioID fishCatch3;
    [SerializeField] [Range(0, .6f)]private float playThreshold3;
    [SerializeField] private AudioID fishCatch4;
    [SerializeField] [Range(0, .8f)]private float playThreshold4;
    [SerializeField] private AudioID fishCatch5;
    [SerializeField] [Range(0, .9f)]private float playThreshold5;
    
    private bool hookLowered;
    private bool rotatingHook;

    private Quaternion fromRotation;
    private Quaternion targetRotation;

    private float current;
    
    //Fish Variables
    
    private int fishCount;
    private float fishCapacityPercentage;
    private SphereCollider netBodyCollider;
    
    // Have the ball follow an anchor;
    
    #region Unity Functions

    private void Awake()
    {
        if (!ropeController) ropeController.GetComponent<RopeController>();
    }

    private void Start()
    {
        if (netBody != null)
        {
            netBodyCollider = netBody.GetComponent<SphereCollider>();
            netBodyCollider.radius = minNetRadius;
        }

        if (fishBall != null)
        {
            fishBall.localScale = Vector3.zero;
        }
    }

    private void Update()
        {
            netTransform.transform.position = ropeController.GetRopePosition(2);
            netBody.transform.position = ropeController.GetRopePosition(3);
            fishBall.transform.position = ropeController.GetRopePosition(3);
        }

        private void FixedUpdate()
        {
            //netBody.MovePosition(anchor.position);

            if (rotatingHook)
            {
                RotateHook(fromRotation, targetRotation);
            }
        }

    #endregion

    #region Hook Functions

        private void RotateHook(Quaternion from, Quaternion to)
        {
            //Increment variable
            current = Mathf.MoveTowards(current, 1, actionSpeed * Time.fixedDeltaTime);
            
            //Lerp hook towards target rotation
            hook.localRotation = Quaternion.Lerp(from, to, hookRotationCurve.Evaluate(current));

            //When alpha is 1, rotatingHook = false;
            if (current >= 1)
            {
                current = 0;
                rotatingHook = false;
                hookLowered = !hookLowered;
                Debug.Log("[Hook State]: " + hookLowered);
            }
        }
        
        public void ActuateHook()
        {
            if (rotatingHook) return;
            
            float targetValue = hookLowered ? neutralRotation : loweredRotation;
            targetRotation = Quaternion.AngleAxis(targetValue, Vector3.right);
            fromRotation = hook.localRotation;
            
            rotatingHook = true;
        }

    public bool IsHookLowered()
    {
        return hookLowered;
    }

    #endregion

    #region Fish Functions

        private IEnumerator SpawnFishBall()
        {
            fishnetTransform.gameObject.SetActive(false);
            FishBall fishball = Instantiate(fishBallPrefab, netTransform.position, Quaternion.identity).GetComponent<FishBall>();
            fishball.SetFillPercentage(fishCapacityPercentage);
            
            yield return new WaitForSeconds(.5f);
            
            fishCount = 0;
            fishCapacityPercentage = 0;
            netBodyCollider.radius = minNetRadius;
            fishBall.localScale = Vector3.zero;
            
            fishnetTransform.gameObject.SetActive(true);
        }
    
        public void DropFishNet()
        {
            StartCoroutine(SpawnFishBall());
            //Play Audio Net DropAudio Here
        }
    
        public bool AddFish()
        {
            if (!fishnetTransform.gameObject.activeInHierarchy) return false;
            if(fishCount >= maxFishCount) return false;
            fishCount++;
            fishAudioCount++;
           
     
            if (fishAudioCount > 5)
            {
            Debug.Log("hallo");
            CallAudio();
            fishAudioCount = 0;
            }


        fishCapacityPercentage = Mathf.Clamp01(fishCount / (float)maxFishCount);
            
            netBodyCollider.radius = Mathf.Lerp(minNetRadius, maxNetRadius, fishCapacityPercentage);
            float fishBallSize = Mathf.Lerp(minFishBallRadius, maxFishBallRadius, fishCapacityPercentage);
            
            fishBall.localScale = Vector3.one * fishBallSize; 
            
            Debug.Log("[NetController]: [Fish Capacity %]" + fishCapacityPercentage);
            
            return true;


      


          //  if (fishCapacityPercentage >= 1)
          //  {
           //     AudioController.Instance.PlayAudio(fishNetFull);
           // }
            //else if (fishCapacityPercentage >= playThreshold5)
           // {
           //     AudioController.Instance.PlayAudio(fishCatch5);
           // }
           // else if (fishCapacityPercentage >= playThreshold4)
          // {
           //     AudioController.Instance.PlayAudio(fishCatch4);
           // }
           // else if (fishCapacityPercentage >= playThreshold3)
          // {
            //    AudioController.Instance.PlayAudio(fishCatch3);
          //  }
           // else if (fishCapacityPercentage >= playThreshold2)
           // {
             //   AudioController.Instance.PlayAudio(fishCatch2);
           // }
           // else if (fishCapacityPercentage >= playThreshold1)
           // {
            //    AudioController.Instance.PlayAudio(fishCatch1);
           // }
        }

        void CallAudio()
        {
        clip = Sounds[UnityEngine.Random.Range(0, Sounds.Length)];
        myAudioSource.PlayOneShot(clip);
        }

    #endregion


        

}
