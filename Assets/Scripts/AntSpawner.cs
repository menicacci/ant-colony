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

    public int anthillIndex;
    public PopulationScore populationScore;

    private void Start() 
    {
        InvokeRepeating("SpawnAnts", 0f, spawnInterval);
    }

    public int GetNumberOfAnts()
    {
        return this.numberOfAnts;
    }

    public int GetFoodFound()
    {
        return this.foodFound;
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
        this.populationScore.ChangeFoodFound(foodFound, this.anthillIndex);
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
        this.populationScore.ChangePopulation(numberOfAnts, this.anthillIndex);
    }

    public void RemoveAnt()
    {
        this.numberOfAnts--;
        this.populationScore.ChangePopulation(numberOfAnts, this.anthillIndex);

    }
}