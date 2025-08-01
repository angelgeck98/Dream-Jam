using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogController : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PlayerInputHandler inputHandler;

    [Header("Dribble Settings")]
    [SerializeField] private float dribbleHeight = 1f;
    [SerializeField] private float dribbleSpeed = 5f;

    [Header("ThrowSettings")]
    [SerializeField] private float throwForce = 15f;

    private Rigidbody rb;
    private Vector3 carryOffset = new Vector3(0, 0, 1f);
    private float dribbleTimer = 0f;

    public enum DogState { Carrying, Dribbling, Thrown }
    public DogState currentState = DogState.Carrying;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        UpdateDog();
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
        }
    }

    void StartDribbling()
    {
        currentState = DogState.Dribbling;
        rb.isKinematic = true;
        dribbleTimer = 0f;
    }

    void StartCarrying()
    {
        currentState = DogState.Carrying;
        rb.isKinematic = true;
    }

    void ThrowDog()
    {
        currentState = DogState.Thrown;
        rb.isKinematic = false; // enables physics


        transform.parent = null; // release from player

        Vector3 throwDirection = playerCamera.transform.forward; // throw in camera's facing direction
        rb.velocity = throwDirection * throwForce;

        // upward force for arc shape
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + 5f, rb.velocity.z);
    }
    
   
}

