using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class GameOverScreen : MonoBehaviour
{
    [Header("Game Over Elements")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Image blackOverlay;

    [Header("Heart Transition")]
    [SerializeField] private Image heartsDisplayUI;
    [SerializeField] private GameObject targetGameObject;

    private float transitionDuration = 0.3f;
    private float delayAfterBlack = 1f;
    private float shakeIntensity = 10f;
    private float shakeDuration = 2f;
    private float delayAfterShake = 0.2f;

  
    private Vector2 targetPosition = new Vector2(25, 60);
    private Vector2 heartUI_Position;
    private float targetRotation = -90f;
    private float heartUI_Rotation = 0;

    private bool isRestarting;
    

    [Header("Death Animation")]
    [SerializeField] private Image animationImage;
    [SerializeField] private Sprite[] animationFrames;
    [SerializeField] private float frameRate = 24f;
    [SerializeField] private int loopFrameCount = 5; // loop last few frames
    // [SerializeField] private bool loopLastFrames = true;
    [SerializeField] private bool playInReverse = false;
    private Coroutine animationCoroutine;
    Vector2 defaultHeartUI_Position = new Vector2(445, 140);

    void Awake()
    {
        InitializeScreen();

    }

    private void InitializeScreen()
    {
        gameOverPanel?.SetActive(false);

        if (blackOverlay != null)
        {
            blackOverlay.gameObject.SetActive(true);
            blackOverlay.color = new Color(0, 0, 0, 0f); // transparent black
        }

        if (animationImage!= null)
        {
            animationImage.enabled = false;
        }

        if (targetGameObject != null)
        {
            RectTransform heartRect = heartsDisplayUI.GetComponent<RectTransform>();
            heartUI_Position = heartRect.anchoredPosition;
            targetPosition = targetGameObject.GetComponent<RectTransform>().anchoredPosition;
            Debug.Log("correct target position: " + targetPosition);
        }
    }


    public void ShowGameOver()
    {
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {

        yield return StartCoroutine(FadeToBlack());
        yield return new WaitForSecondsRealtime(delayAfterBlack);
        yield return StartCoroutine(ShakeEgg(shakeDuration, shakeIntensity));
        yield return new WaitForSecondsRealtime(delayAfterShake);
        yield return StartCoroutine(MoveEgg(targetPosition, targetRotation, transitionDuration));
        Debug.Log("target position: " + targetPosition);
        
        yield return StartCoroutine(PlayDeathAnimation());

        RestartGame();
    }

    public void RestartGame()
    {
        if (isRestarting) return;
        isRestarting = true;
        StartCoroutine(RestartSequence());
    }

    private IEnumerator RestartSequence()
    {
        //gameOverPanel?.SetActive(false);


        Debug.Log("attempting to restart");
        if (animationCoroutine != null)
        { 
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }

        yield return StartCoroutine(PlayDeathAnimation(true));
        animationImage.gameObject.SetActive(false);
        heartsDisplayUI.gameObject.SetActive(true);
        yield return StartCoroutine(MoveEgg(defaultHeartUI_Position, heartUI_Rotation, transitionDuration));

        isRestarting = false;
        yield return StartCoroutine(FadeFromBlack());
        Debug.Log("SceneManager: Ready to restart");

    }

    public IEnumerator ShakeEgg(float shakeDuration, float shakeIntensity)
    {
        float duration = shakeDuration;
        float intensity = shakeIntensity;

        if (heartsDisplayUI == null) yield break;

        RectTransform heartUI_Rect = heartsDisplayUI.rectTransform;
        Vector2 originalPosition = heartUI_Rect.anchoredPosition;

        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * shakeIntensity;
            heartUI_Rect.anchoredPosition = originalPosition + randomOffset;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        heartUI_Rect.anchoredPosition = originalPosition;
    }

    private IEnumerator FadeToBlack()
    {
        if (blackOverlay == null) yield break;

        blackOverlay.color = Color.black;
        yield return null;
    }

    private IEnumerator FadeFromBlack()
    {
        if (blackOverlay == null) yield break;

        float fadeDuration = 0.5f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = 1f - (elapsed / fadeDuration);
            blackOverlay.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        blackOverlay.color = new Color(0, 0, 0, 0f);
    }

    private IEnumerator MoveEgg(Vector2 targetPos, float targetRot, float duration)
    {
        if (heartsDisplayUI == null) yield break;

        RectTransform heartUI_Rect = heartsDisplayUI.rectTransform;
        Vector2 debug_pos = heartUI_Rect.anchoredPosition;
        Debug.Log("Moving egg from " + debug_pos + "to " + targetPos);

        LeanTween.move(heartUI_Rect, targetPos, duration).setIgnoreTimeScale(true);
        LeanTween.rotateZ(heartUI_Rect.gameObject, targetRot, duration).setIgnoreTimeScale(true);

        yield return new WaitForSecondsRealtime(duration);
    }

    private IEnumerator PlayDeathAnimation(bool playInReverse = false)
    {
        heartsDisplayUI.gameObject.SetActive(false);
        animationImage.gameObject.SetActive(true);

        float frameDuration = 1f / frameRate;

        if (playInReverse)
        {
            for (int i = animationFrames.Length - 1; i >= 0; i--)
            {
                animationImage.sprite = animationFrames[i];
                yield return new WaitForSecondsRealtime(frameDuration);
            }
        }
        else
        {
            for (int i = 0; i < animationFrames.Length; i++)
            {
                animationImage.sprite = animationFrames[i];
                yield return new WaitForSecondsRealtime(frameDuration);
            }

            int startLoopIndex = Mathf.Max(0, animationFrames.Length - loopFrameCount);
            //debug:
            float max = 2;
            float count = 0;

            while (count < max)
            {
                for (int i = startLoopIndex; i < animationFrames.Length; i++)
                {
                    animationImage.sprite = animationFrames[i];
                    yield return new WaitForSecondsRealtime(frameDuration);
                }
                count++;
            }
        }
    }

}