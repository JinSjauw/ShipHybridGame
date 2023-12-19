using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [SerializeField] private MeshRenderer OceanRenderer;
    
    [SerializeField] private Vector4 WaveA;
    [SerializeField] private Vector4 WaveB;
    [SerializeField] private Vector4 WaveC;
    //[SerializeField] private Vector4 WaveD;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.Log("Instance already exists");
            Destroy(this);
        }

        OceanRenderer.sharedMaterial.SetVector("_WaveA", WaveA);
        OceanRenderer.sharedMaterial.SetVector("_WaveB", WaveB);
        OceanRenderer.sharedMaterial.SetVector("_WaveC", WaveC);
    }

    private Vector3 GerstnerWave(Vector4 wave, Vector3 point)
    {
        float steepness = wave.z;
        float wavelength = wave.w;
        float k = 2 * Mathf.PI / wavelength;
        float c = Mathf.Sqrt(9.8f / k);
        Vector2 d = new Vector2(wave.x, wave.y).normalized;
        float f = k * (Vector2.Dot(d, new Vector2(point.x, point.z)) - c * Time.time);
        float a = steepness / k;

        return new Vector3(
            d.x * (a * Mathf.Cos(f)), 
            a * Mathf.Sin(f),
            d.y * (a * Mathf.Cos(f)));
    }
    
    public Vector3 FindPoint(Vector2 point)
    {
        //Do calculations here
        Vector3 pointInWave = new Vector3(point.x, 0, point.y);
        pointInWave += GerstnerWave(WaveA, pointInWave);
        pointInWave += GerstnerWave(WaveB, pointInWave);
        pointInWave += GerstnerWave(WaveC, pointInWave);

        return pointInWave;
    }
}
