using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour, IDamageable
{
    public int Health { get; set; } = 100;

    public void Damage(int amount)
    {
        // Empty implementation - player doesn't damage others
        // Or you could add logic here if the player can damage things
    }

    public void TakeDamage(int amount)
    {
        Health -= amount;
        Debug.Log("Player Health: " + Health);

        if (Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has Died!");
        //Game Logic
    }
}
