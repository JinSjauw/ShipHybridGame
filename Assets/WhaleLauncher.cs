using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhaleLauncher : MonoBehaviour
{
    [SerializeField] private List<Transform> projectileList;

    [SerializeField] private float launchDelay;
    [SerializeField] private float spawnHeight;
    [SerializeField] private bool debug;
    
    private int launchAmount;
    private Transform target;
    
    private IEnumerator LaunchProjectile()
    {
        for (int i = 0; i < launchAmount; i++)
        {
            Vector3 targetPosition = target.position;
            targetPosition.y = spawnHeight;
            Log("Fired Projectile #" + i);
            int prefabIndex = Random.Range(0, projectileList.Count - 1);
            Instantiate(projectileList[prefabIndex], targetPosition, Quaternion.identity);
            yield return new WaitForSeconds(launchDelay);
        }
    }

    private void Log(string _msg)
    {
        if (!debug) return;
        Debug.Log("[WhaleLauncher]: " + _msg);
    }
    
    public void Launch(int amount, Transform targetTransform)
    {
        Log("LaunchAmount:"  + launchAmount);
        launchAmount = amount;
        target = targetTransform;
        StartCoroutine(LaunchProjectile());
    }
}
