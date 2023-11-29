using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AntController : MonoBehaviour
{
    public Transform holdSpot;

    public LayerMask tanaMask;
    public LayerMask pickUpMask;
    public LayerMask homeMask;
    public LayerMask foodMask;
    public LayerMask antMask;

    public ParticleSystem foodPheromone;
    public ParticleSystem homePheromone;

    public float moveSpeed;
    public float rotationSpeed;
    public float collisionRadius;
    public float perceptionRadius;
    public float foodRadius;

    private Rigidbody rb;
    private Vector3 targetDirection;
    private GameObject food;
    private bool hasFood = false;
    private Transform targetFood;
    
    private float timer = 0f;
    private float changeDirectionTimer;
    public float antLifetime = 60f;

    public GameObject pheromoneDetector;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        changeDirectionTimer = Random.Range(2.0f, 5.0f);


        ActivatePheromoneSystem();
        UpdatePheromoneSystem();
        RandomDirection();
    }

    private void Update()
    {
        ParticleSystem pheromone = hasFood ? foodPheromone : homePheromone;

        // Update timer
        changeDirectionTimer -= Time.deltaTime;
        antLifetime -= Time.deltaTime;
        timer += Time.deltaTime;

        if (antLifetime <= 0)
        {
            Death();
        }

        if (!hasFood) 
        {
            if (targetFood == null)
            {
                LookForFood();
            }
            else
            {
                PickUpFood();
            }
        }
        else
        {
            ReturnHome();
        }

        if (changeDirectionTimer <= 0)
        {
            RandomDirection();
        }

        // Move the ant
        rb.transform.Translate(targetDirection * moveSpeed * Time.deltaTime, Space.World);
        Rotate();
    }

    private void LookForFood() 
    {      
        Collider[] targetedFood = Find(pickUpMask, perceptionRadius);
        if (targetedFood.Length > 0) 
        {
            targetFood = targetedFood[Random.Range(0, targetedFood.Length)].transform;
            targetDirection = (targetFood.position - transform.position).normalized;
            changeDirectionTimer = 7.5f;

            // Debug.Log("Food found");
        }
        else
        {
            if (changeDirectionTimer <= 0)
            {
                FollowTrails(true);
            }            
        }

    }

    private class ParticleInfo
    {
        public Vector3 position;
        public float remainingLifetime;

        public ParticleInfo(Vector3 pos, float lifetime)
        {
            position = pos;
            remainingLifetime = lifetime;
        }
    }

    private void FollowTrails(bool isSearching)
    {
       Collider[] nearByAnts = Find(antMask, perceptionRadius);

       if (nearByAnts.Length > 0)
       {
               List<ParticleSystem> particleSystem = FilterParticles(nearByAnts, isSearching);
              
               if (particleSystem.Count > 0)
               {
                      List<ParticleInfo> pheromoneDetected = FindBestParticles(particleSystem);

                      if (pheromoneDetected.Count > 0)
                      {
                            Vector3 position = GetTargetPosition(pheromoneDetected);

                            targetDirection = (position - transform.position).normalized;
                            changeDirectionTimer = 2f;
                      }
               }            
       }
    }

    private List<ParticleSystem> FilterParticles(Collider[] ants, bool isSearching)
    {
        List<ParticleSystem> particlesFound = new List<ParticleSystem>();
    
        int pheromoneSystem = isSearching ? 0 : 1;
        foreach (Collider ant in ants)
        {
            ParticleSystem[] particles = ant.gameObject.GetComponentsInChildren<ParticleSystem>();
    
            if (particles != null && particles.Length == 2 && particles[pheromoneSystem].isEmitting)
            {
                particlesFound.Add(particles[pheromoneSystem]);
            }
        }
    
        return particlesFound;
    }

    private List<ParticleInfo> FindBestParticles(List<ParticleSystem> particleSystem)
    {
        List<ParticleInfo> pheromoneDetected = new List<ParticleInfo>();

        foreach (ParticleSystem ps in particleSystem)
        {                            
              // Get the particles from the particle system
              ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];

              int numParticlesAlive = ps.GetParticles(particles);

              for (int i = 0; i < Mathf.Min(numParticlesAlive, 20); i++)
              {
                  // Accessing position information
                  Vector3 particlePosition = particles[i].position;
                  float particleLifetime = particles[i].remainingLifetime;

                  pheromoneDetected.Add(new ParticleInfo(particlePosition, particleLifetime));
              }
        }

        return pheromoneDetected;
    }

    private Vector3 GetTargetPosition(List<ParticleInfo> pheromoneDetected)
    {
        float totalWeight = 0.0f;
        Vector3 weightedSum = Vector3.zero;

        foreach (ParticleInfo particle in pheromoneDetected)
        {
            float weight = 1.0f / (particle.remainingLifetime + 1.0f); // Adding 1 to avoid division by zero
            weightedSum += particle.position * weight;
            totalWeight += weight;
        }

        return weightedSum / totalWeight;
    }

    private void PickUpFood()
    {
        Collider[] pickedUpFood = Find(pickUpMask, foodRadius);

        if (pickedUpFood.Length > 0) 
        {
            food = pickedUpFood[0].gameObject;
            food.transform.position = holdSpot.position;
            food.transform.parent = transform;
            food.GetComponent<Rigidbody>().isKinematic = true;

            food.layer = LayerMask.NameToLayer("GoingHome");
            hasFood = true;

            changeDirectionTimer = 0;
            UpdatePheromoneSystem();
            ReturnHome();
        }
    }

    // TODO: Logic for returning home
    private void ReturnHome() 
    {        
        if (changeDirectionTimer <= 0)
        {           
            Collider[] home = Find(tanaMask, perceptionRadius);
            if (home.Length > 0) 
            {
                targetDirection = (home[0].transform.position - transform.position).normalized;
                changeDirectionTimer = Random.Range(0f, 2.0f);
            }
            else {
                FollowTrails(false);
            }
            
        }
        else
        {
            Collider[] home = Find(tanaMask, foodRadius);

            if (home.Length > 0) 
            {
                DropOffFood();
            }
        }
    }

    // TODO: Handle ants' death
    private void Death() 
    {
        // Debug.Log("Time finished");
    }

    private void DropOffFood()
    {
        Destroy(food);
        hasFood = false;
        targetFood = null;
        changeDirectionTimer = 0;

        UpdatePheromoneSystem();
        LookForFood();
    }

    private void RandomDirection()
    {
        // Generate a new random direction
        targetDirection = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));
        targetDirection.Normalize();
    
        // Reset the timer for the next direction change
        changeDirectionTimer = Random.Range(2.0f, 5.0f);

        targetFood = null;
    }

    private void Rotate() 
    {
        if (targetDirection != Vector3.zero) 
        {
            // Rotate towards the current movement direction
            Quaternion toRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            rb.transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private Collider[] Find(LayerMask mask, float radius)
    {
        return Physics.OverlapSphere(transform.position + targetDirection, radius, mask);
    }

    private void ActivatePheromoneSystem()
    {
        foodPheromone.gameObject.SetActive(true);
        homePheromone.gameObject.SetActive(true);
    }

    private void UpdatePheromoneSystem()
    {
        timer = 0;
        
        ParticleSystem pheromoneToEmit = hasFood ? foodPheromone : homePheromone;
        ParticleSystem pheromoneToStop = !hasFood ? foodPheromone : homePheromone;

        pheromoneToEmit.Play();
        pheromoneToStop.Stop();
    }
}
