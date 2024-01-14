using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingSpotTest : MonoBehaviour
{
    [SerializeField] private float percentageWaveHeight = 0.75f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = transform.position;
        Vector3 wavePosition = WaveManager.Instance.FindPoint(new Vector2(position.x, position.z));

        position.y = wavePosition.y * percentageWaveHeight;
        transform.position = position;
    }
}
