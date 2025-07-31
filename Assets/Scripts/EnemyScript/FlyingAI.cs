using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlyingAI : MonoBehaviour
{
  
    private NavMeshAgent m_Agent;
    private Animator m_Animator;
    private Transform m_Target;

    public float MoveSpeed;
    public float FlyingHeight;

    private void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
        m_Target = GameObject.FindGameObjectWithTag("Player").transform;
        
        /*
         Random flapping wings animation timing 
         float randomAnimSpeed = Random.Range(0.1f, 0.5f); 
         float newAnimSpeed = Mathf.Round(randomAnimSpeed * 10)/ 10;
         m_animator.speed = newAnimSpeed; 
         */
        m_Agent.speed = MoveSpeed;
        m_Agent.baseOffset = FlyingHeight;

    }

    private void Update()
    {
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        m_Agent.destination = m_Target.position;
    }
}
