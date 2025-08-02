using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DogController : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int dogDamageAmount = 25;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PlayerInputHandler inputHandler;

    [Header("Dribble Settings")]
    [SerializeField] private float dribbleHeight = 1f;
    [SerializeField] private float dribbleSpeed = 5f;

    [Header("Throw Settings")]
    [SerializeField] private float throwForce = 15f;
    [SerializeField] private float upwardArc = 5f;

    [Header("Return Settings")]
    [SerializeField] private float returnDistance = 2f;
    [SerializeField] private float returnDelay = 1f;
    [SerializeField] private float returnSpeed = 5f;

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
        if (navAgent != null) navAgent.enabled = false;
    }

    void Update()
    {
        HandleInput();
        UpdateDogState();
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

    void UpdateDogState()
    {
        switch (currentState)
        {
            case DogState.Carrying:
                transform.position = player.position + player.TransformDirection(carryOffset);
                transform.rotation = player.rotation;
                break;

            case DogState.Dribbling:
                dribbleTimer += Time.deltaTime * dribbleSpeed;
                float bounceY = Mathf.Abs(Mathf.Cos(dribbleTimer)) * dribbleHeight;

                Vector3 dribbleStart = player.position + player.TransformDirection(carryOffset);
                transform.position = new Vector3(dribbleStart.x, dribbleStart.y - 1f + bounceY, dribbleStart.z);
                break;

            case DogState.Thrown:
                // handled by physics
                break;

            case DogState.Returning:
                if (navAgent.enabled)
                {
                    navAgent.SetDestination(player.position);
                    if (Vector3.Distance(transform.position, player.position) < returnDistance)
                    {
                        StartCarrying();
                    }
                }
                break;
        }
    }

    void StartDribbling()
    {
        currentState = DogState.Dribbling;
        dribbleTimer = 0f;
        rb.isKinematic = true;

        if (navAgent != null) navAgent.enabled = false;
    }

    public void StartCarrying()
    {
        currentState = DogState.Carrying;
        transform.parent = player;
        rb.isKinematic = true;

        if (navAgent != null) navAgent.enabled = false;
    }

    void ThrowDog()
    {
        currentState = DogState.Thrown;
        transform.parent = null;
        rb.isKinematic = false;

        if (navAgent != null) navAgent.enabled = false;

        Vector3 throwDirection = playerCamera.transform.forward.normalized;
        rb.velocity = throwDirection * throwForce + Vector3.up * upwardArc;

        Debug.Log("Dog thrown: " + rb.velocity);
    }

    void StartReturning()
    {
        currentState = DogState.Returning;
        rb.isKinematic = true;

        if (navAgent != null)
        {
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                navAgent.enabled = true;
                navAgent.speed = returnSpeed;
                navAgent.SetDestination(player.position);
            }
            else
            {
                Debug.LogWarning("Dog too far from NavMesh — teleporting...");
                if (NavMesh.SamplePosition(player.position, out NavMeshHit playerHit, 5f, NavMesh.AllAreas))
                {
                    transform.position = playerHit.position;
                    navAgent.enabled = true;
                    navAgent.SetDestination(player.position);
                }
                else
                {
                    Debug.LogError("Could not find a valid NavMesh position.");
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (currentState == DogState.Thrown)
        {
            Debug.Log("Dog hit: " + collision.gameObject.name);

            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(dogDamageAmount);
                Debug.Log("Dog dealt damage: " + dogDamageAmount);
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
