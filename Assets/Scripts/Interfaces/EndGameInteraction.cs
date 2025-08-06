using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndGameInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioClip fishSound;
    [SerializeField] private float volume = 1f;

    public void Interact()
    {
        Debug.Log("I interacted with fish");

        if (fishSound != null)
        {
            StartCoroutine(PlaySoundAndRestart());
        }
        else
        {
            StartCoroutine(RestartGame());
        }
    }

    private IEnumerator PlaySoundAndRestart()
    {
        // Play the sound through SoundFXManager
        SoundFXManager.instance.PlaySoundFX(fishSound, transform, volume);

        // Wait for the sound to finish playing
        yield return new WaitForSeconds(fishSound.length);

        // Then restart the scene
        yield return RestartGame();
    }

    private IEnumerator RestartGame()
    {
        Time.timeScale = 1f; // in case game is paused
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        yield return null;
    }
}