using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXObject; // Prefab for one-shots
    private AudioSource currentLoopingAudioSource; // Only tracks the AudioSource, not the bird

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

    // One-shot sounds (unchanged)
    public void PlaySoundFX(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        // Create at position but DON'T parent to spawnTransform
        AudioSource audioSource = Instantiate(
            soundFXObject, 
            spawnTransform.position, 
            Quaternion.identity
        );
        
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
        
        // Destroy only the audio object, not the parent
        Destroy(audioSource.gameObject, audioClip.length + 0.1f);
    }

    // Looping sounds now create a dedicated child object
    public AudioSource PlayLoopingSound(AudioClip audioClip, Transform parentTransform, float volume)
    {
        StopLoopingSound(); // Stop any existing loop first

        // Create a new GameObject just for the sound (child of the bird)
        GameObject soundObj = new GameObject("FlappingSound");
        soundObj.transform.SetParent(parentTransform); // Attach to bird
        soundObj.transform.localPosition = Vector3.zero; // Center on bird

        // Add and configure AudioSource
        AudioSource newSource = soundObj.AddComponent<AudioSource>();
        newSource.clip = audioClip;
        newSource.volume = volume;
        newSource.loop = true;
        newSource.Play();
        
        
        return newSource;

    }

    // Safely stops ONLY the looping sound (not the bird)
    
    public void StopLoopingSound()
    {
        if (currentLoopingAudioSource != null && currentLoopingAudioSource.gameObject != null)
        {
            Destroy(currentLoopingAudioSource.gameObject); // Only kills the sound object
            currentLoopingAudioSource = null;
        }
    }
    
}