using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameOverScreen gameOverScreen;

    [Header("Player Reference")]
    [SerializeField] private PlayerDamage playerDamage;
    
    [Header("Lives Display Options")]
    [SerializeField] private bool useHeartIcons = false;
    [SerializeField] private Image heartDisplayImage;
    [SerializeField] private Sprite[] heartSprites;


    private void Start()
    {
        // Try to find PlayerDamage if not assigned
        if (playerDamage == null)
        {
            playerDamage = FindObjectOfType<PlayerDamage>();
            if (playerDamage == null)
            {
                Debug.LogError("PlayerDamage script not found! Make sure it's attached to a GameObject in the scene.");
                return;
            }
        }
        
        // Subscribe to player events
        playerDamage.OnLivesChanged.AddListener(UpdateLivesUI);
        playerDamage.OnGameOver.AddListener(ShowGameOver);
        playerDamage.OnPlayerHit.AddListener(OnPlayerHit); // Optional: for hit effects
        
        // Hide game over panel initially
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        UpdateLivesUI(playerDamage.Lives);
    }
    
    private void UpdateLivesUI(int currentLives)
    {
        // Update lives debug text
        if (livesText != null)
        {
            livesText.text = "Lives: " + currentLives;
        }

        // Update heart icons if using them
        if (heartDisplayImage != null && heartSprites != null && heartSprites.Length > 0)
        {  
            // reverse index and ensure array won't out of bounds
            int spriteIndex = (heartSprites.Length) - currentLives;
            spriteIndex = Mathf.Clamp(spriteIndex, 0, heartSprites.Length - 1);
            heartDisplayImage.sprite = heartSprites[spriteIndex];
            Debug.Log($"Lives: {currentLives}, Using sprite index: {spriteIndex}");
        }
    }
    
    private void ShowGameOver()
    {
        Time.timeScale = 0f;
        gameOverScreen.ShowGameOver();
    }
    
    private void OnPlayerHit()
    {
        // Optional: Add hit effects here
        // Examples: screen shake, red flash, hit sound, etc.
        Debug.Log("Player was hit! UI can show hit effects here.");
    }
    
    // Call this from a restart button
    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        if (playerDamage != null)
        {
            playerDamage.OnLivesChanged.RemoveListener(UpdateLivesUI);
            playerDamage.OnGameOver.RemoveListener(ShowGameOver);
            playerDamage.OnPlayerHit.RemoveListener(OnPlayerHit);
        }
    }
}