using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntSpawner : MonoBehaviour
{
    public GameObject antPrefab;
    public int initialAnts = 100;
    public float spawnInterval = 1.0f;
    public int foodSpawnRatio = 3;

    public int foodFound = 0;
    public int numberOfAnts = 0;

    private void Start() 
    {
        InvokeRepeating("SpawnAnts", 0f, spawnInterval);
    }

    private void SpawnAnts()
    {
        if (initialAnts > 0)
        {
            SpawnAnt();
            initialAnts--;
        }
        else
        {
            // Stop spawning once all ants are created.
            CancelInvoke("SpawnAnts");
        }
    }

    public void IncrementFood()
    {
        foodFound++;
        if (foodFound % this.foodSpawnRatio == 0)
        {
            SpawnAnt();
        }
    }

    private void SpawnAnt()
    {
        GameObject antObject = Instantiate(antPrefab, transform.position, Quaternion.identity);
        antObject.transform.parent = transform;

        AntController ant = antObject.GetComponent<AntController>();
        ant.Initialize(this);
    }

    public void AddAnt()
    {
        this.numberOfAnts++;
    }

    public void RemoveAnt()
    {
        this.numberOfAnts--;
    }
}