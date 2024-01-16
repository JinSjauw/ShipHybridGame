using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhaleLauncher : MonoBehaviour
{
    [SerializeField] private List<Transform> projectileList;

    [SerializeField] private float launchDelay;
    [SerializeField] private bool debug;
    
    private int launchAmount;
    
    private IEnumerator LaunchProjectile()
    {
        for (int i = 0; i < launchAmount; i++)
        {
            Log("Fired Projectile #" + i);
            yield return new WaitForSeconds(launchDelay);
        }
    }

    private void Log(string _msg)
    {
        if (!debug) return;
        Debug.Log("[WhaleLauncher]: " + _msg);
    }
    
    public void Launch(int amount)
    {
        Log("LaunchAmount:"  + launchAmount);
        launchAmount = amount;
        StartCoroutine(LaunchProjectile());
    }
}
