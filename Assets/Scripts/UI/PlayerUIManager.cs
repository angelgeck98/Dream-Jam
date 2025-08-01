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
    
    [Header("Player Reference")]
    [SerializeField] private PlayerDamage playerDamage;
    
    [Header("Lives Display Options")]
    [SerializeField] private bool useHeartIcons = false;
    [SerializeField] private Image[] heartImages; // Optional: for heart icons
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

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
    }
    
    private void UpdateLivesUI(int currentLives)
    {
        // Update lives text
        if (livesText != null)
            livesText.text = "Lives: " + currentLives;
        
        // Update heart icons if using them
        if (useHeartIcons && heartImages != null)
        {
            for (int i = 0; i < heartImages.Length; i++)
            {
                if (heartImages[i] != null)
                {
                    if (i < currentLives)
                        heartImages[i].sprite = fullHeart;
                    else
                        heartImages[i].sprite = emptyHeart;
                }
            }
        }
    }
    
    private void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            // Optional: Pause the game
            Time.timeScale = 0f;
        }
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