using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumGenerator : MonoBehaviour, IInteractable, IDamageable
{
  public int Health { get; set; } = 1;

  public void Interact()
  {
    Debug.Log(Random.Range(0,100));
  }


  
  public void Damage(int amount)
  {
    Health -= amount;
    
    if (Health <= 0)
    {
      Die();
    }
  }

  public void TakeDamage(int damageAmount)
  {
    Damage(damageAmount);
  }

  private void Die()
  {
    Destroy(gameObject);
  }
  
}
