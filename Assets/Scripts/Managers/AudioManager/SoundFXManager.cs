using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXObject;
    
    // Track the current looping sound GameObject
    private GameObject currentLoopingSound;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // One-shot sounds (your existing method)
    public void PlaySoundFX(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume; 
        audioSource.Play();
        
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }
    
    // Looping sound using the same soundFXObject approach
    public void PlayLoopingSound(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        // Stop any existing loop first
        StopLoopingSound();
        
        // Create new looping sound using the same pattern
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.loop = true; // Enable looping
        audioSource.Play();
        
        // Track this GameObject so we can stop it later
        currentLoopingSound = audioSource.gameObject;
        
        Debug.Log($"Started looping sound: {audioClip.name}");
    }
    
    // Stop the current looping sound
    public void StopLoopingSound()
    {
        if (currentLoopingSound != null)
        {
            Destroy(currentLoopingSound);
            currentLoopingSound = null;
            Debug.Log("Stopped looping sound");
        }
    }
    
    // Check if looping sound is playing
    public bool IsLoopingPlaying()
    {
        return currentLoopingSound != null;
    }
}