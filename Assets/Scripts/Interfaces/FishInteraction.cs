using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishInteraction : MonoBehaviour, IInteractable
{

    [SerializeField] private AudioClip fishSound;
    [SerializeField] private float volume = 1f;
    
  
    
    public void Interact()
    {
        
        if (fishSound != null)
        {
            SoundFXManager.instance.PlaySoundFX(fishSound, transform, volume);
        }
        
    }
}
