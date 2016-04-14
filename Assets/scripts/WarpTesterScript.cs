using UnityEngine;
using System.Collections;
using System;

public class WarpTesterScript : MonoBehaviour
{
    public GameObject toSpawn;
    public float SpawnRate = 5;
    private float t;

    void Start()
    {

    }
    
    void Update()
    {
        t += Time.deltaTime;
        if (t > 1 / SpawnRate)
        {
            Spawn();
            t = 0;
        }
    }

    private void Spawn()
    {
        Instantiate(toSpawn, new Vector3(UnityEngine.Random.value * 32, 0, UnityEngine.Random.value * 32), Quaternion.Euler(90, 0, 0));
    }
}
