using System.Collections;
using UnityEngine;

public class DogController : MonoBehaviour
{
    [SerializeField] private AudioClip throwingSoundClip;
    [SerializeField] private AudioClip dribblingSoundClip;
    
    [Header("Damage Settings")]
    [SerializeField] private int dogDamageAmount = 25;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PlayerInputHandler inputHandler;

    [Header("Follow Settings")]
    [SerializeField] private float followSpeed = 15f;
    [SerializeField] private float rotationSpeed = 12f;

    [Header("Dribble Settings")]
    [SerializeField] private float dribbleHeight = 1f;
    [SerializeField] private float dribbleSpeed = 5f;
    [SerializeField] private float dribbleFollowSpeed = 8f;

    [Header("Throw Settings")]
    [SerializeField] private float throwForce = 15f;
    [SerializeField] private float upwardArc = 5f;
    [SerializeField] private float throwCooldown = 0.5f; // Prevent rapid throwing

    private Rigidbody rb;
    public Vector3 carryOffset = new Vector3(0, -0.5f, 1.5f);
    private float dribbleTimer = 0f;
    private float lastThrowTime = 0f;

    public enum DogState { Carrying, Dribbling, Thrown }
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
        // Start dribbling when button is held
        if (inputHandler.DribbleHeld && currentState == DogState.Carrying)
        {
            StartDribbling();
        }

        // Stop dribbling when button is released
        if (!inputHandler.DribbleHeld && currentState == DogState.Dribbling)
        {
            StartCarrying();
        }

        // Throw when button is pressed while dribbling (with cooldown)
        if (inputHandler.ThrowTriggered && currentState == DogState.Dribbling && 
            Time.time - lastThrowTime > throwCooldown)
        {
            ThrowDog();
        }
    }

    void UpdateDogState()
    {
        switch (currentState)
        {
            case DogState.Carrying:
                // Direct position following (more responsive)
                Vector3 targetPosition = player.position + player.TransformDirection(carryOffset);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);
                
                // Smooth rotation following
                transform.rotation = Quaternion.Slerp(transform.rotation, player.rotation, rotationSpeed * Time.deltaTime);
                break;

            case DogState.Dribbling:
                // Bouncing motion
                dribbleTimer += Time.deltaTime * dribbleSpeed;
                float bounceY = Mathf.Abs(Mathf.Sin(dribbleTimer)) * dribbleHeight;

                Vector3 dribbleBase = player.position + player.TransformDirection(carryOffset);
                Vector3 dribblePosition = new Vector3(dribbleBase.x, dribbleBase.y - 1f + bounceY, dribbleBase.z);
                
                // Follow player while dribbling
                transform.position = Vector3.MoveTowards(transform.position, dribblePosition, dribbleFollowSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, player.rotation, rotationSpeed * 0.7f * Time.deltaTime);
                break;

            case DogState.Thrown:
                // Physics handles this state - dog is airborne
                break;
        }
    }

    void StartDribbling()
    {
        currentState = DogState.Dribbling;
        dribbleTimer = 0f;
        rb.isKinematic = true;
        transform.parent = null;

        if (dribblingSoundClip != null)
        {
            SoundFXManager.instance.PlayLoopingSound(dribblingSoundClip, transform, 0.5f);
        }
    }

    public void StartCarrying()
    {
        currentState = DogState.Carrying;
        rb.isKinematic = true;
        
        // Stop dribbling sound
        if (SoundFXManager.instance != null)
        {
            SoundFXManager.instance.StopLoopingSound();
        }
    }

    void ThrowDog()
    {
        currentState = DogState.Thrown;
        lastThrowTime = Time.time;
        transform.parent = null;
        rb.isKinematic = false;
        
        // Stop dribbling sound and play throw sound
        if (SoundFXManager.instance != null)
        {
            SoundFXManager.instance.StopLoopingSound();
            if (throwingSoundClip != null)
            {
                SoundFXManager.instance.PlaySoundFX(throwingSoundClip, transform, 0.3f);
            }
        }
        
        // Calculate throw direction - more predictable
        Vector3 throwDirection = playerCamera.transform.forward;
        Vector3 throwVelocity = throwDirection * throwForce + Vector3.up * upwardArc;
        
        rb.velocity = throwVelocity;
        
        Debug.Log("Dog thrown with velocity: " + rb.velocity);
    }

    // This method can be called by the player when they pick up the dog
    public void PickUp()
    {
        if (currentState == DogState.Thrown)
        {
            StartCarrying();
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

            // Check if it's a basketball hoop or scoring area by tag
            if (collision.gameObject.CompareTag("BasketballHoop"))
            {
                // You can add your scoring logic here
                Debug.Log("SCORE! Dog hit the basketball hoop!");
                
                // Example: Send message to a game manager
                // GameManager.instance.AddScore(100);
            }
        }
    }

    // Optional: Add a method to reset the dog if it gets stuck
    public void ResetToPlayer()
    {
        transform.position = player.position + player.TransformDirection(carryOffset);
        StartCarrying();
    }
}