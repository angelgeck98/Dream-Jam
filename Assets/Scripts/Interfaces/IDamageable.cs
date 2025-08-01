using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    int Health { get; set; }
    void Damage(int amount); //For damaging others
    void TakeDamage(int damageAmount); //For receiving damage
}
