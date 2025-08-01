using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int damageAmount = 10;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Enemy collided with: " + collision.gameObject.name);

        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damageAmount); // ✅ now using TakeDamage
            Debug.Log("Enemy dealt damage: " + damageAmount);
        }
    }
}
