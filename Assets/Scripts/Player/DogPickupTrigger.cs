using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogPickupTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        DogController dogController = other.GetComponentInParent<DogController>();

        if (dogController != null && dogController.currentState == DogController.DogState.Thrown)
        {
            Debug.Log(other);
            dogController.StartCarrying();
        }
    }
}
