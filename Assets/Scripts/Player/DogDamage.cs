using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogDamage : MonoBehaviour
{

    public int damageAmount;
    
    private void OnCollisionEnter(Collision collision)
    {
        
        Debug.Log("Collision happened with: " + collision.gameObject.name);
        
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        if (collision.gameObject && damageable != null)
        {
            damageable.Damage(damageAmount);
        }
        

    }
}
