using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AITarget : MonoBehaviour
{
    public Transform Target;
    public float AttackDistance;
    public float speed = 10f;

    private NavMeshAgent m_Agent;

    private Animator m_Animator;

    private float m_distance;
    

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        m_distance = Vector3.Distance(m_Agent.transform.position, Target.position);
        m_Agent.destination = Target.position * (speed * Time.deltaTime);
        /*
        if (m_distance < AttackDistance)
        {
            m_Agent.isStopped = true;
            m_Animator.SetBool("Attack", true);
        }
        else
        {
         m_Agent.isStopped = false;
          m_Animator.SetBool("Attack", false);
          m_Agent.destination = Target.position
        }
        */


    }
}
