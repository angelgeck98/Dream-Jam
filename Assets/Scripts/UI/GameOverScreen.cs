using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    [Header("Game Over Elements")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Image blackOverlay;

    [Header("Heart Transition")]
    [SerializeField] private Image heartsDisplayUI;
    [SerializeField] private GameObject targetGameObject;

    [Header("Timing Settings")]
    [SerializeField] private float transitionDuration = 0.3f;
    [SerializeField] private float delayAfterBlack = 1f;
    [SerializeField] private float shakeIntensity = 10f;
    [SerializeField] private float shakeDuration = 2f;
    [SerializeField] private float delayAfterShake = 0.2f;

    [Header("Animation Settings")]
    [SerializeField] private Image deathAnimImage;
    [SerializeField] private Sprite[] deathAnimFrames;
    [SerializeField] private Image gameOverAnimImage;
    [SerializeField] private Sprite[] gameOverAnimFrames;
    [SerializeField] private float frameRate = 24f;
    [SerializeField] private int loopFrameCount = 5;

    [Header("Debug Options")]
    [SerializeField] private bool showRestartSequence = false;
    [SerializeField] private float gameOverDisplayDuration = 3f; // How long to show game over before restart

    // Private variables
    private Vector2 targetPosition;
    private Vector2 heartUIPosition;
    //private Vector2 defaultHeartUIPosition = new Vector2(445, 140);
    private float targetRotation = -90f;
    private float heartUIRotation = 0f;
    private bool isRestarting;
    private Coroutine deathAnimationCoroutine;
    private Coroutine gameOverAnimationCoroutine;
    private Image restartButtonImage;

    #region Unity Lifecycle
    void Awake()
    {
        InitializeScreen();
    }
    #endregion

    #region Initialization
    private void InitializeScreen()
    {
        Debug.Log("");
        SetupRestartButton();
        //SetupBlackOverlay();
        //SetupAnimationImages();
        SetupHeartPositions();
    }

    private void SetupRestartButton()
    {

        if (restartButton != null)
        {
            //Debug.Log("attempting to set up button");

            // Get the button's image component
            restartButtonImage = restartButton.GetComponent<Image>();

            // Setup button click event
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartGame);

            // Add hover functionality
            var eventTrigger = restartButton.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = restartButton.gameObject.AddComponent<EventTrigger>();
            }

            // Clear existing triggers
            eventTrigger.triggers.Clear();

            // Add hover enter
            var pointerEnter = new EventTrigger.Entry();
            pointerEnter.eventID = EventTriggerType.PointerEnter;
            pointerEnter.callback.AddListener((data) => OnRestartButtonHover(true));
            eventTrigger.triggers.Add(pointerEnter);

            // Add hover exit
            var pointerExit = new EventTrigger.Entry();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((data) => OnRestartButtonHover(false));
            eventTrigger.triggers.Add(pointerExit);

            // Initially disable the button
            restartButton.gameObject.SetActive(false);
        }
    }

    private void SetupBlackOverlay()
    {

        if (blackOverlay != null)
        {
            blackOverlay.gameObject.SetActive(true);
            blackOverlay.color = new Color(0, 0, 0, 0f);
        }
    }

    private void SetupAnimationImages()
    {
        if (deathAnimImage != null)
            deathAnimImage.enabled = false;

        if (gameOverAnimImage != null)
            gameOverAnimImage.enabled = false;
    }

    private void SetupHeartPositions()
    {
        if (targetGameObject != null && heartsDisplayUI != null)
        {  
            RectTransform heartRect = heartsDisplayUI.GetComponent<RectTransform>();
            heartUIPosition = heartRect.anchoredPosition;
            targetPosition = targetGameObject.GetComponent<RectTransform>().anchoredPosition;
            Debug.Log($"Target position set to: {targetPosition}");
        }
    }

    #endregion


    #region Public Methods
    public void ShowGameOver()
    {
        StartCoroutine(GameOverSequence());
    }

    public void RestartGame()
    {
        if (isRestarting) return;
        isRestarting = true;
        StartCoroutine(RestartSequence());
    }
    #endregion

    #region Game Over Sequence
    private IEnumerator GameOverSequence()
    {
        yield return StartCoroutine(FadeToBlack());
        yield return new WaitForSecondsRealtime(delayAfterBlack);

        yield return StartCoroutine(ShakeEgg(shakeDuration, shakeIntensity));
        yield return new WaitForSecondsRealtime(delayAfterShake);

        yield return StartCoroutine(MoveEgg(targetPosition, targetRotation, transitionDuration));

        yield return StartCoroutine(PlayDeathAnimationOnce());

        // Start both animations: death animation looping last frames + game over animation
        deathAnimationCoroutine = StartCoroutine(PlayDeathAnimationLoop());
        gameOverAnimationCoroutine = StartCoroutine(PlayGameOverAnimationLoop());

        EnableRestartButton();

        // If we want to show the restart sequence, wait a bit then restart automatically
        if (showRestartSequence)
        {
            yield return new WaitForSecondsRealtime(gameOverDisplayDuration);
            RestartGame();
        }
    }
    #endregion

    #region Restart Sequence
    private IEnumerator RestartSequence()
    {
        Debug.Log("Starting restart sequence");

        DisableRestartButton();
        StopAllAnimations();


        gameOverAnimImage.gameObject.SetActive(false);
        yield return StartCoroutine(PlayDeathAnimationReverse());


        ResetAnimationImages();
        ResetHeartDisplay();
        yield return StartCoroutine(MoveEgg(heartUIPosition, heartUIRotation, transitionDuration));

        isRestarting = false;
        yield return StartCoroutine(FadeFromBlack());

        Debug.Log("Restart sequence complete");
    }

    private void StopAllAnimations()
    {
        if (deathAnimationCoroutine != null)
        {
            StopCoroutine(deathAnimationCoroutine);
            deathAnimationCoroutine = null;
        }

        if (gameOverAnimationCoroutine != null)
        {
            StopCoroutine(gameOverAnimationCoroutine);
            gameOverAnimationCoroutine = null;
        }
    }

    private void ResetAnimationImages()
    {
        if (deathAnimImage != null)
            deathAnimImage.gameObject.SetActive(false);

        if (gameOverAnimImage != null)
            gameOverAnimImage.gameObject.SetActive(false);
    }

    private void ResetHeartDisplay()
    {
        if (heartsDisplayUI != null)
            heartsDisplayUI.gameObject.SetActive(true);
    }
    #endregion

    #region Visual Effects
    public IEnumerator ShakeEgg(float duration, float intensity)
    {
        if (heartsDisplayUI == null) yield break;

        RectTransform heartUIRect = heartsDisplayUI.rectTransform;
        Vector2 originalPosition = heartUIRect.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * intensity;
            heartUIRect.anchoredPosition = originalPosition + randomOffset;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        heartUIRect.anchoredPosition = originalPosition;
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

        RectTransform heartUIRect = heartsDisplayUI.rectTransform;
        Vector2 currentPos = heartUIRect.anchoredPosition;
        Debug.Log($"Moving egg from {currentPos} to {targetPos}");

        LeanTween.move(heartUIRect, targetPos, duration).setIgnoreTimeScale(true);
        LeanTween.rotateZ(heartUIRect.gameObject, targetRot, duration).setIgnoreTimeScale(true);

        yield return new WaitForSecondsRealtime(duration);
    }
    #endregion

    #region Animation Methods
    private IEnumerator PlayDeathAnimationOnce()
    {
        if (deathAnimFrames == null || deathAnimFrames.Length == 0) yield break;

        heartsDisplayUI.gameObject.SetActive(false);
        deathAnimImage.gameObject.SetActive(true);

        float frameDuration = 1f / frameRate;

        // Play animation once
        for (int i = 0; i < deathAnimFrames.Length; i++)
        {
            deathAnimImage.sprite = deathAnimFrames[i];
            yield return new WaitForSecondsRealtime(frameDuration);
        }
    }

    private IEnumerator PlayDeathAnimationReverse()
    {
        if (deathAnimFrames == null || deathAnimFrames.Length == 0) yield break;

        float frameDuration = 1f / frameRate;

        for (int i = deathAnimFrames.Length - 1; i >= 0; i--)
        {
            deathAnimImage.sprite = deathAnimFrames[i];
            yield return new WaitForSecondsRealtime(frameDuration);
        }
    }



    private IEnumerator PlayGameOverAnimationLoop()
    {
        if (gameOverAnimFrames == null || gameOverAnimFrames.Length == 0) yield break;

        gameOverAnimImage.color = new Color(1f, 1f, 1f, 0f);
        gameOverAnimImage.gameObject.SetActive(true);

        float frameDuration = 1f / (frameRate + 24f);
        StartCoroutine(FadeInGameOverAnimation());


        while (true)
        {
            for (int i = 0; i < gameOverAnimFrames.Length; i++)
            {
                gameOverAnimImage.sprite = gameOverAnimFrames[i];
                yield return new WaitForSecondsRealtime(frameDuration);
            }
        }
    }

    private IEnumerator FadeInGameOverAnimation()
    {
        float elapsed = 0f;
        float fadeDuration = 0.5f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            gameOverAnimImage.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        // Ensure it's fully opaque
        gameOverAnimImage.color = new Color(1f, 1f, 1f, 1f);
    }

    private IEnumerator PlayDeathAnimationLoop()
    {
        if (deathAnimFrames == null || deathAnimFrames.Length == 0) yield break;

        float frameDuration = 1f / frameRate;
        int startLoopIndex = Mathf.Max(0, deathAnimFrames.Length - loopFrameCount);

        while (true)
        {
            for (int i = startLoopIndex; i < deathAnimFrames.Length; i++)
            {
                deathAnimImage.sprite = deathAnimFrames[i];
                yield return new WaitForSecondsRealtime(frameDuration);
            }
        }
    }
    #endregion

        #region UI Management
    private void EnableRestartButton()
    {
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(true);
            OnRestartButtonHover(false);
        }

        // Enable cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void DisableRestartButton()
    {
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false);
        }

        // Disable cursor (assuming you want it hidden during gameplay)
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnRestartButtonHover(bool isHovering)
    {
        if (restartButtonImage != null)
        {
            // Show sprite on hover, hide when not hovering
            restartButtonImage.color = isHovering ? Color.white : Color.clear;
        }
    }
    #endregion
}