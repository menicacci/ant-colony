using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntSpawner : MonoBehaviour
{
    public GameObject antPrefab;
    public int numberOfAnts = 100;
    public float spawnInterval = 1.0f;

    private void Start() 
    {
        InvokeRepeating("SpawnAnts", 0f, spawnInterval);
    }

    private void SpawnAnts()
    {
        if (numberOfAnts > 0)
        {
            GameObject ant = Instantiate(antPrefab, transform.position, Quaternion.identity);
            ant.transform.parent = transform;
            numberOfAnts--;
        }
        else
        {
            // Stop spawning once all ants are created.
            CancelInvoke("SpawnAnts");
        }
    }

}