using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : MonoBehaviour, IDamageable, IInteractable
{
    [Header("Enemy Health Settings")]
    [SerializeField] private int maxHealth = 1;
    
    public int Health { get; set; }
    
    private void Start()
    {
        Health = maxHealth;
    }

    public void Damage(int amount)
    {
        // Empty - enemies don't damage other enemies
    }

    public void TakeDamage(int amount)
    {
        Health -= amount;
        Debug.Log(gameObject.name + " Health: " + Health);

        if (Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " has died!");
        
        // Add death effects here (particles, sound, etc.)
        
        // Destroy the enemy
        Destroy(gameObject);
    }
    
    public void Interact()
    {
        Debug.Log(Random.Range(0,100));
    }
    
}
