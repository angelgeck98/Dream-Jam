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
    
    
    [SerializeField] private AudioClip flyingSoundClip;
    [SerializeField] private AudioClip flappingSoundClip;
    public bool hasStartedFollowing = false;
   
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
      
       
        
        
    float distToPlayer = Vector3.Distance(transform.position, m_Target.position);
    if (distToPlayer < 1.0f)
    {
        // TODO: Call damage function
        Debug.Log("Player in attack range!");
    }
    }

    private void MoveToTarget()
    {
        
        
        if (m_Target == null) return;

   
        Vector3 dir = (m_Target.position - transform.position).normalized;

       
        Vector3 offset = dir * -2.5f; // negative to stay ~1.5 units away from player

        Vector3 adjustedTarget = m_Target.position + offset;

       
        adjustedTarget.y = m_Target.position.y + FlyingHeight;

        m_Agent.destination = adjustedTarget;

       
    }


    // ReSharper disable Unity.PerformanceAnalysis
    private void BeginFollowing()
    {
        if (!hasStartedFollowing)
        {
            hasStartedFollowing = true;
            if (flappingSoundClip)
            {
               
                SoundFXManager.instance.PlayLoopingSound(flappingSoundClip, transform, 0.7f);
                
            }
        }
    }

    public void OnSpawned()
    {
        
        Debug.Log("OnSpawned called on " + gameObject.name);
        
        if (flyingSoundClip)
        {
            SoundFXManager.instance.PlaySoundFX(flyingSoundClip, transform, 0.5f);
        }

        BeginFollowing();

    }
}
