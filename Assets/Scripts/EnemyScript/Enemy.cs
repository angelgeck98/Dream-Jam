using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player; 
    public LayerMask groundLayer, playerLayer;
    public float health;
    public float sightRange;
    public float attackRange;
    public int damage;

    public Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        player = GameObject.Find("player").transform; 
        
    }
    
    
    // Update is called once per frame
    void Update()
    {
        bool playerInSightRange = Physics.CheckSphere(transform.position, sightRange, groundLayer);
        bool playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
        }
        else if (playerInAttackRange && playerInSightRange)
        {
            AttackPlayer();
        }
    }

    void ChasePlayer()
    {
        
    }

    void AttackPlayer()
    {
        
    }
}
