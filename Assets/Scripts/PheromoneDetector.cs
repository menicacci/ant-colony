using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PheromoneDetector : MonoBehaviour
{
    private List<GameObject> collidedParticles = new List<GameObject>();

    private void Update()
    {
        Debug.Log(collidedParticles.Count);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject collidedObject = collision.gameObject;
        
        // Check if the collided object has the "Particle" tag
        if (collidedObject.CompareTag("Pheromone"))
        {
            collidedParticles.Add(collidedObject);
        }
    }

    // Function to retrieve the list of collided particles
    public List<GameObject> GetCollidedParticles()
    {
        return collidedParticles;
    }
}
