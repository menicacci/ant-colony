using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodGeneration : MonoBehaviour {

    public float radius;
    public GameObject food;
    public int amount;
    public int nSpawnCenter = 3;
    Vector3[] spawnCenters;

    public LayerMask groundLayer;

    void Awake () 
    {
        spawnCenters = new Vector3[nSpawnCenter];
        
        spawnCenters[0] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        for (int i = 1; i < nSpawnCenter; i++) 
        {
            spawnCenters[i] = transform.position + Random.insideUnitSphere * radius;
        }

        for (int i = 0; i < amount; i++) 
        {
            SpawnFood();
        }
    }

    void SpawnFood () 
    {
        Vector3 randomPointInSphere = Random.insideUnitSphere * Mathf.Min(Random.value, Random.value) * 50;
        Vector3 centre = spawnCenters[Random.Range(0, nSpawnCenter - 1)] + randomPointInSphere;

        centre.y = Terrain.activeTerrain.SampleHeight(transform.position) + 100;
        // Perform a raycast to find the ground position.
        RaycastHit hit;
        if (Physics.Raycast(centre, Vector3.down, out hit, Mathf.Infinity, groundLayer)) 
        {
            centre = hit.point;
        }

        Instantiate(food, centre, Quaternion.identity, transform);
    }
}
