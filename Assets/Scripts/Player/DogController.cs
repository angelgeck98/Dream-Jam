using System.Collections;
using UnityEngine;

public class DogController : MonoBehaviour
{
    [SerializeField] private AudioClip throwingSoundClip;
    [SerializeField] private AudioClip dribblingSoundClip;
    [SerializeField] private AudioClip returningSoundClip;

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
    [SerializeField] private float returnSpeed = 8f;
    [SerializeField] private float returnRotationSpeed = 5f;

    private Rigidbody rb;
    public Vector3 carryOffset = new Vector3(0, 0, 1f);
    private float dribbleTimer = 0f;

    public enum DogState { Carrying, Dribbling, Thrown, Returning }
    public DogState currentState = DogState.Carrying;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
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
                // Direct movement back to player - like a basketball rolling back
                Vector3 targetPosition = player.position + player.TransformDirection(carryOffset);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, returnSpeed * Time.deltaTime);
                
                // Smooth rotation toward player
                Vector3 directionToPlayer = (player.position - transform.position).normalized;
                if (directionToPlayer != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, returnRotationSpeed * Time.deltaTime);
                }
                
                // Check if close enough to start carrying
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
        dribbleTimer = 0f;
        rb.isKinematic = true;

        if (dribblingSoundClip != null)
        {
            SoundFXManager.instance.PlayLoopingSound(dribblingSoundClip, transform, 0.5f);
        }
    }

    public void StartCarrying()
    {
        currentState = DogState.Carrying;
        transform.parent = player;
        rb.isKinematic = true;
        
        // Stop any return sounds
        if (SoundFXManager.instance != null)
        {
            SoundFXManager.instance.StopLoopingSound();
        }
    }

    void ThrowDog()
    {
        currentState = DogState.Thrown;
        transform.parent = null;
        rb.isKinematic = false;
        
        if (SoundFXManager.instance != null)
        {
            SoundFXManager.instance.StopLoopingSound();
            if (throwingSoundClip != null)
            {
                SoundFXManager.instance.PlaySoundFX(throwingSoundClip, transform, 0.3f);
            }
        }
        
        Vector3 throwDirection = playerCamera.transform.forward.normalized;
        rb.velocity = throwDirection * throwForce + Vector3.up * upwardArc;

        Debug.Log("Dog thrown: " + rb.velocity);
    }

    void StartReturning()
    {
        if (currentState == DogState.Returning)
        {
            return;
        }
        
        currentState = DogState.Returning;
        rb.isKinematic = true; // Stop physics, use direct movement
        
        if (returningSoundClip != null && SoundFXManager.instance != null)
        {
            SoundFXManager.instance.PlayLoopingSound(returningSoundClip, transform, 0.7f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (currentState == DogState.Thrown)
        {
            Debug.Log("Dog hit: " + collision.gameObject.name);

            // Deal damage if the object can take damage
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

    // Optional: Gizmo to visualize return distance
    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(player.position, returnDistance);
        }
    }
}