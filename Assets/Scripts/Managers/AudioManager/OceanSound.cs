using UnityEngine;

public class OceanAmbientSound : MonoBehaviour
{
    [Header("Ocean Audio")]
    [SerializeField] private AudioClip oceanSoundClip;
    [SerializeField] private AudioClip songSoundClip;
    [SerializeField] private float oceanVolume = 50f;
    [SerializeField] private float songVolume = 20f;
    
    [Header("Distance Settings")]
    [SerializeField] private bool useDistanceVolume = true;
    [SerializeField] private float maxDistance = 50f;
    [SerializeField] private float minVolume = 0.05f;
    
    private Transform player;
    private AudioSource oceanAudioSource;
    private AudioSource songAudioSource;
    private bool isPlaying = false;

    void Start()
    {
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // Create separate AudioSources for each sound
        SetupAudioSources();
        
        // Start playing both sounds
        PlayAmbientSounds();
    }

    private void SetupAudioSources()
    {
        // Ocean waves AudioSource
        if (oceanSoundClip != null)
        {
            oceanAudioSource = gameObject.AddComponent<AudioSource>();
            oceanAudioSource.clip = oceanSoundClip;
            oceanAudioSource.loop = true;
            oceanAudioSource.volume = oceanVolume;
            oceanAudioSource.spatialBlend = 1f; // 3D sound
            oceanAudioSource.rolloffMode = AudioRolloffMode.Linear;
            oceanAudioSource.maxDistance = maxDistance;
        }

        // Song AudioSource  
        if (songSoundClip != null)
        {
            songAudioSource = gameObject.AddComponent<AudioSource>();
            songAudioSource.clip = songSoundClip;
            songAudioSource.loop = true;
            songAudioSource.volume = songVolume;
            songAudioSource.spatialBlend = 0f; // 2D sound (plays everywhere)
            // For music, you might want spatialBlend = 0f so it plays at consistent volume
        }
    }

    private void PlayAmbientSounds()
    {
        if (oceanAudioSource != null)
        {
            oceanAudioSource.Play();
        }
        
        if (songAudioSource != null)
        {
            songAudioSource.Play();
        }
        
        isPlaying = true;
    }

    void Update()
    {
        if (useDistanceVolume && player != null && isPlaying)
        {
            UpdateVolumeByDistance();
        }
    }

    private void UpdateVolumeByDistance()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        float volumePercent = Mathf.Clamp01(1f - (distance / maxDistance));
        
        // Update ocean volume based on distance
        if (oceanAudioSource != null)
        {
            float adjustedOceanVolume = Mathf.Lerp(minVolume, oceanVolume, volumePercent);
            oceanAudioSource.volume = adjustedOceanVolume;
        }
        
        // Song might stay constant or also fade with distance
        if (songAudioSource != null)
        {
            float adjustedSongVolume = Mathf.Lerp(minVolume * 0.5f, songVolume, volumePercent);
            songAudioSource.volume = adjustedSongVolume;
        }
    }

    void OnDestroy()
    {
        if (oceanAudioSource != null)
        {
            oceanAudioSource.Stop();
        }
        
        if (songAudioSource != null)
        {
            songAudioSource.Stop();
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