using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntSpawner : MonoBehaviour
{
    public GameObject antPrefab;
    public int numberOfAnts = 100;
    public float spawnInterval = 1.0f;

    private int foodFound = 0;

    private void Start() 
    {
        InvokeRepeating("SpawnAnts", 0f, spawnInterval);
    }

    private void SpawnAnts()
    {
        if (numberOfAnts > 0)
        {
            GameObject antObject = Instantiate(antPrefab, transform.position, Quaternion.identity);
            antObject.transform.parent = transform;

            AntController ant = antObject.GetComponent<AntController>();
            ant.Initialize(this);

            numberOfAnts--;
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
        Debug.Log(foodFound);
    }

}