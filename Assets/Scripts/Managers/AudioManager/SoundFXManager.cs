using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXObject;
    private AudioSource currentLoopingAudioSource;

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

    public void PlaySoundFX(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(audioSource.gameObject, audioClip.length + 0.1f);
    }

    public AudioSource PlayLoopingSound(AudioClip audioClip, Transform parentTransform, float volume)
    {
        StopLoopingSound();

        GameObject soundObj = new GameObject("FlappingSound");
        soundObj.transform.SetParent(parentTransform);
        soundObj.transform.localPosition = Vector3.zero;

        AudioSource newSource = soundObj.AddComponent<AudioSource>();
        newSource.clip = audioClip;
        newSource.volume = volume;
        newSource.loop = true;
        newSource.Play();
        
        currentLoopingAudioSource = newSource;
        return newSource;
    }

    public void StopLoopingSound()
    {
        if (currentLoopingAudioSource != null && currentLoopingAudioSource.gameObject != null)
        {
            Destroy(currentLoopingAudioSource.gameObject);
            currentLoopingAudioSource = null;
        }
    }
}