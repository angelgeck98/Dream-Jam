using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerDamage : MonoBehaviour, IDamageable
{
    [Header("Lives Settings")]
    [SerializeField] private int maxLives = 3;
    
    public int Health { get; set; } = 1; // Keep this for interface, but not used
    public int Lives { get; private set; }
    
    [Header("Events")]
    public UnityEvent<int> OnLivesChanged;
    public UnityEvent OnPlayerHit;
    public UnityEvent OnGameOver;
    
    [Header("Hit Settings")]
    [SerializeField] private float invincibilityTime = 1f; // Brief invincibility after being hit
    [SerializeField] private float fallDamageYLevel = 3.5f;
    private bool isInvincible = false;
    private bool hasFallen = false;

    private void Start()
    {
        Lives = maxLives;
        Debug.Log("Player started with " + Lives + " lives");
        
        // Notify UI of initial value
        OnLivesChanged?.Invoke(Lives);
    }
    private void Update()
    {
        // Check for fall damage
        if (transform.position.y <= fallDamageYLevel && hasFallen == false)
        {
            Debug.Log("Player fell below Y level " + fallDamageYLevel + "! Taking fall damage.");
            hasFallen = true;
            TakeDamage(30);
        }
    }


    public void Damage(int amount)
    {
        // Empty - player doesn't damage others directly
    }

    public void TakeDamage(int amount)
    {
        // Ignore damage if invincible (prevents multiple hits at once)
        if (isInvincible) 
        {
            Debug.Log("Player is invincible, ignoring damage");
            return;
        }
        if (amount >= 30) Lives = 0;
        else { Lives--; }
 
    
        Debug.Log("Player hit! Lives remaining: " + Lives);
        
        // Fire events
        OnLivesChanged?.Invoke(Lives);
        OnPlayerHit?.Invoke();
        
        if (Lives <= 0)
        {
            // Game Over
            Debug.Log("Game Over! No lives remaining!");
            OnGameOver?.Invoke();
        }
        else
        {
            // Brief invincibility to prevent multiple hits
            StartCoroutine(InvincibilityPeriod());
        }
    }
    
    private IEnumerator InvincibilityPeriod()
    {
        isInvincible = true;
        Debug.Log("Player is now invincible for " + invincibilityTime + " seconds");
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
        Debug.Log("Player invincibility ended");
    }
    
    // Public methods for other scripts
    public void AddLife()
    {
        if (Lives < maxLives) // Optional: cap at max lives
        {
            Lives++;
            Debug.Log("Life added! Lives: " + Lives);
            OnLivesChanged?.Invoke(Lives);
        }
    }
    
    public bool IsGameOver()
    {
        return Lives <= 0;
    }
}