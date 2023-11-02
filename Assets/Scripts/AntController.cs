using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntController : MonoBehaviour
{
    public Transform holdSpot;
    public LayerMask pickUpMask;

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
    
    private float changeDirectionTimer;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        changeDirectionTimer = Random.Range(2.0f, 5.0f);
    
        RandomDirection();
    }

    private void Update()
    {
        // Update timer
        changeDirectionTimer -= Time.deltaTime;

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
        Collider[] targetedFood = findFood(perceptionRadius);

            if (targetedFood.Length > 0) 
            {
                targetFood = targetedFood[Random.Range(0, targetedFood.Length)].transform;
                targetDirection = (targetFood.position - transform.position).normalized;
                changeDirectionTimer = 5f;
            }
    }

    private void PickUpFood()
    {
        Collider[] pickedUpFood = findFood(foodRadius);

        if (pickedUpFood.Length > 0) 
        {
            food = pickedUpFood[0].gameObject;
            food.transform.position = holdSpot.position;
            food.transform.parent = transform;
            food.GetComponent<Rigidbody>().isKinematic = true;

            food.layer = LayerMask.NameToLayer("GoingHome");
            hasFood = true;

            ReturnHome();
        }
    }

    // TODO: Logic for returning home
    private void ReturnHome() {
        RandomDirection();
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

    private void Rotate() {
        if (targetDirection != Vector3.zero) 
        {
            // Rotate towards the current movement direction
            Quaternion toRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            rb.transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private Collider[] findFood(float radius) 
    {
        return Physics.OverlapSphere(transform.position + targetDirection, radius, pickUpMask);
    }
}
