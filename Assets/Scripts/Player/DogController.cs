using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DogController : MonoBehaviour
{

    [Header("References")] [SerializeField] 
    private int dogDamageAmount = 25;
    
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PlayerInputHandler inputHandler;

    [Header("Dribble Settings")]
    [SerializeField] private float dribbleHeight = 1f;
    [SerializeField] private float dribbleSpeed = 5f;

    [Header("ThrowSettings")]
    [SerializeField] private float throwForce = 15f;
    
    [Header("ReturnSettings")]
    [SerializeField] private float returnSpeed = 5f;
    [SerializeField] private float returnDistance = 5f;
    [SerializeField] private float returnDelay = 1f; 

    private Rigidbody rb;
    private NavMeshAgent navAgent;
    private Vector3 carryOffset = new Vector3(0, 0, 1f);
    private float dribbleTimer = 0f;

    public enum DogState { Carrying, Dribbling, Thrown, Returning }
    public DogState currentState = DogState.Carrying;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        navAgent = GetComponent<NavMeshAgent>();
        
        rb.isKinematic = true;

        if (navAgent != null)
        {
            navAgent.enabled = false;
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        UpdateDog();
        
        if (currentState == DogState.Returning && navAgent != null && navAgent.enabled)
        {
            navAgent.SetDestination(player.position);
        }
    }

    void HandleInput()
    {
        if (inputHandler.DribbleHeld && currentState == DogState.Carrying)
        {
            StartDribbling();
        }

        if (!inputHandler.DribbleHeld && currentState == DogState.Dribbling)
        {
            StartCarrying();
        }

        if (inputHandler.ThrowTriggered && currentState == DogState.Dribbling)
        {
            ThrowDog();
        }
    }

    void UpdateDog()
    {
        switch (currentState)
        {
            case DogState.Carrying:
                transform.position = player.position + player.TransformDirection(carryOffset);
                transform.rotation = player.rotation;
                break;

            case DogState.Dribbling:
                // bounce dog with cos wave (starts at top)
                dribbleTimer += Time.deltaTime * dribbleSpeed;
                float bounceY = Mathf.Abs(Mathf.Cos(dribbleTimer)) * dribbleHeight;

                Vector3 dribbleStartPosition = player.position + player.TransformDirection(carryOffset);
                transform.position = new Vector3(dribbleStartPosition.x, dribbleStartPosition.y - 1f + bounceY, dribbleStartPosition.z);
                break;

            case DogState.Thrown:
                break;
            
            case DogState.Returning:
                if (Vector3.Distance(transform.position, player.position) < returnDistance)
                {
                    StartCarrying();
                }
                break;
        }
    }

    void StartDribbling()
    {
        currentState = DogState.Dribbling;
        rb.isKinematic = true;

        if (navAgent != null)
        {
            navAgent.enabled = false;
        }
        
        dribbleTimer = 0f;
    }

    public void StartCarrying()
    {
        currentState = DogState.Carrying;
        rb.isKinematic = true;
        transform.parent = player;
        
        if (navAgent != null)
        {
            navAgent.enabled = false;
        }
    }

    void ThrowDog()
    {
        currentState = DogState.Thrown;
        rb.isKinematic = false; // enables physics
        transform.parent = null; // release from player
        
        if (navAgent != null)
        {
            navAgent.enabled = false;
        }

        Vector3 throwDirection = playerCamera.transform.forward; // throw in camera's facing direction
        rb.velocity = throwDirection * throwForce;

        // upward force for arc shape
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + 5f, rb.velocity.z);
    }

    void StartReturning()
    {
        currentState = DogState.Returning;

        rb.isKinematic = true;

        if (navAgent != null)
        {
          if(NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
          {
              navAgent.enabled = true;
              transform.position = hit.position;
              navAgent.speed = returnSpeed;
              navAgent.SetDestination(player.position);
          }
          else
          {
              Debug.LogWarning("Dog too far way, teleporting to it");
              if (NavMesh.SamplePosition(player.position, out NavMeshHit playerNavHit, 5f, NavMesh.AllAreas))
              {
                  transform.position = playerNavHit.position;
                  navAgent.enabled = true;
                  navAgent.SetDestination(player.position);
              }
              else
              {
                  Debug.LogError("No NavMesh");
              }
              
              
          }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (currentState == DogState.Thrown)
        {
            Debug.Log("Thrown dog hit: " + collision.gameObject.name);
            
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(dogDamageAmount);
                Debug.Log("Dog Dealth: " + dogDamageAmount + " damage!");
            }

            StartCoroutine(ReturnAfterDelay());
        }
    }

    private IEnumerator ReturnAfterDelay()
    {
        yield return new WaitForSeconds(returnDelay);
        StartReturning();
    }

   
}