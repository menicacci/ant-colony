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
    private bool dead = false;
    
    private float timer = 0f;
    private float changeDirectionTimer;
    public float antLifetime = 90f;
    private float foodAward = 0f;

    private AntSpawner anthill;
    private Animator animator;

    public void Initialize(AntSpawner antSpawner)
    {
        anthill = antSpawner;
        anthill.AddAnt();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        ActivatePheromoneSystem();
        UpdatePheromoneSystem();
        changeDirectionTimer = Random.Range(0.0f, 0.5f);

        this.foodAward = this.antLifetime/2;
        antLifetime += Random.Range(0f, this.antLifetime/2);

        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Update timer
        changeDirectionTimer -= Time.deltaTime;
        antLifetime -= Time.deltaTime;
        timer += Time.deltaTime;

        if (antLifetime <= 0 && !dead)
        {
            Death();
        }
        else if (!dead)
        {
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
    }

    private void LookForFood() 
    {      
        Collider[] targetedFood = Find(pickUpMask, perceptionRadius);
        if (targetedFood.Length > 0) 
        {
            targetFood = targetedFood[Random.Range(0, targetedFood.Length)].transform;
            targetDirection = (targetFood.position - transform.position).normalized;
            changeDirectionTimer = Random.Range(6.5f, 7.5f);
        }
        else
        {
            if (changeDirectionTimer <= 0)
            {
                FollowTrails(true);
            }            
        }

    }

    private void FollowTrails(bool isSearching)
    {
       Collider[] nearByAnts = Find(antMask, perceptionRadius);
       Collider thisCollider = GetComponent<Collider>();
       
       if (thisCollider != null && !nearByAnts.Contains(thisCollider))
       {
           nearByAnts = nearByAnts.Concat(new Collider[] { thisCollider }).ToArray();
       }

       if (nearByAnts.Length > 0)
       {              
               Vector3 position = AntUtils.Instance.GetTargetPosition(nearByAnts, isSearching, anthill);

               if (position != Vector3.zero)
               {
                      targetDirection = (position - transform.position).normalized;
                      changeDirectionTimer = Random.Range(1.5f, 3.0f);
               }
       }
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

            food.layer = LayerMask.NameToLayer("Ant");
            hasFood = true;

            changeDirectionTimer = 0;
            UpdatePheromoneSystem();
            ReturnHome();
        }
    }

    private void ReturnHome() 
    {        
        if (changeDirectionTimer <= 0)
        {           
            FindHome();
        }

        if (changeDirectionTimer <= 0)
        {
            FollowTrails(false);
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

    private void FindHome()
    {
        Collider[] homes = Find(tanaMask, perceptionRadius);

        if (homes.Length > 0) 
        {
            foreach (Collider homeCollider in homes)
            {
                AntSpawner antSpawner = homeCollider.gameObject.GetComponent<AntSpawner>();
                if (antSpawner != null)
                {
                    if (antSpawner == anthill)
                    {
                        targetDirection = (homeCollider.transform.position - transform.position).normalized;
                        changeDirectionTimer = 15f;
                        
                        break;
                    }
                }
            }
        }
    }

    private void Death() 
    {
        this.dead = true;
        if (hasFood)
        {
            Destroy(food);
        }

        this.foodPheromone.Stop();
        this.homePheromone.Stop();

        anthill.RemoveAnt();
        animator.SetBool("Death", true); 
        
        Invoke("DestroyAnt", 30f);
    }

    private void DestroyAnt()
    {
        Destroy(gameObject);
    }

    private void DropOffFood()
    {
        Destroy(food);
        hasFood = false;
        targetFood = null;
        changeDirectionTimer = 0;

        anthill.IncrementFood();

        UpdatePheromoneSystem();
        LookForFood();

        // Increment lifetime
        this.antLifetime += this.foodAward;
    }

    private void RandomDirection()
    {
        // Generate a new random direction
        targetDirection = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));
        targetDirection.Normalize();
    
        // Reset the timer for the next direction change
        changeDirectionTimer = Random.Range(3.0f, 6.0f);

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
        this.SetPheromoneSystem(true);
    }

    private void DisactivatePheromoneSystem()
    {
        this.SetPheromoneSystem(false);
    }

    private void SetPheromoneSystem(bool isActive)
    {
        foodPheromone.gameObject.SetActive(isActive);
        homePheromone.gameObject.SetActive(isActive);
    }

    private void UpdatePheromoneSystem()
    {
        timer = 0;
        
        ParticleSystem pheromoneToEmit = hasFood ? foodPheromone : homePheromone;
        ParticleSystem pheromoneToStop = !hasFood ? foodPheromone : homePheromone;

        pheromoneToEmit.Play();
        pheromoneToStop.Stop();
    }

    public AntSpawner GetAntHill()
    {
        return anthill;
    }
}
