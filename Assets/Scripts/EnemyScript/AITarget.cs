using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AITarget : MonoBehaviour
{
    public Transform Target;
    public float AttackDistance = 2f;
    public float speed = 3.5f;

    private NavMeshAgent m_Agent;
    private Animator m_Animator;
    private float m_distance;

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();

        if (m_Agent != null)
        {
            m_Agent.speed = speed;
        }
    }

    void Update()
    {
        if (Target == null || m_Agent == null) return;

        m_distance = Vector3.Distance(transform.position, Target.position);

        if (m_distance < AttackDistance)
        {
            m_Agent.isStopped = true;
            if (m_Animator != null)
                m_Animator.SetBool("Attack", true);
        }
        else
        {
            m_Agent.isStopped = false;
            m_Agent.destination = Target.position;
            if (m_Animator != null)
                m_Animator.SetBool("Attack", false);
        }
    }
}