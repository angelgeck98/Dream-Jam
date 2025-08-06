using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanAmbientSound : MonoBehaviour
{
    [Header("Ocean Audio")]
    [SerializeField] private AudioClip oceanSoundClip;
    [SerializeField] private float volume = 0.3f;
    
    [Header("Distance Settings")]
    [SerializeField] private bool useDistanceVolume = true;
    [SerializeField] private float maxDistance = 50f; // Max distance to hear ocean
    [SerializeField] private float minVolume = 0.05f; // Minimum volume when far
    
    private Transform player;
    private bool isPlaying = false;

    void Start()
    {
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // Start playing ocean sound
        if (oceanSoundClip != null && SoundFXManager.instance != null)
        {
            SoundFXManager.instance.PlayLoopingSound(oceanSoundClip, transform, volume);
            isPlaying = true;
        }
    }

    void Update()
    {
        // Adjust volume based on distance to player (optional)
        if (useDistanceVolume && player != null && isPlaying)
        {
            UpdateVolumeByDistance();
        }
    }

    private void UpdateVolumeByDistance()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        float volumePercent = Mathf.Clamp01(1f - (distance / maxDistance));
        float adjustedVolume = Mathf.Lerp(minVolume, volume, volumePercent);
        
        // Note: You'd need to modify SoundFXManager to support volume changes
        // For now, this is just the calculation
    }

    // Stop ocean sound when destroyed
    void OnDestroy()
    {
        if (isPlaying && SoundFXManager.instance != null)
        {
            SoundFXManager.instance.StopLoopingSound();
        }
    }

    // Gizmo to visualize max hearing distance
    private void OnDrawGizmosSelected()
    {
        if (useDistanceVolume)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, maxDistance);
        }
    }
}