using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float flyingHeight = 3f;
    public float followDistance = 2.5f;
    public float rotationSpeed = 5f;
    
    [Header("Audio")]
    [SerializeField] private AudioClip flyingSoundClip;
    [SerializeField] private AudioClip flappingSoundClip;

    private AudioSource _flappingAudioSource;
    
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 1.0f;
    
    private Animator m_Animator;
    private Transform m_Target;
    private bool hasStartedFollowing = false;

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Target = GameObject.FindGameObjectWithTag("Player").transform;
        
        // Optional: Random animation speed for wing flapping variety
        if (m_Animator != null)
        {
            float randomAnimSpeed = Random.Range(0.8f, 1.2f);
            m_Animator.speed = randomAnimSpeed;
        }
    }

    private void Update()
    {
        if (hasStartedFollowing)
        {
            MoveToTarget();
            CheckAttackRange();
        }
    }
    
    private void OnDestroy()
    {
        if (_flappingAudioSource != null)
        {
            Destroy(_flappingAudioSource.gameObject);
        }
    }

    private void MoveToTarget()
    {
        if (m_Target == null) return;

        // Calculate target position (behind player at flying height)
        Vector3 playerDirection = m_Target.forward;
        Vector3 targetPosition = m_Target.position - (playerDirection * followDistance);
        targetPosition.y = m_Target.position.y + flyingHeight;

        // Move towards target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Rotate to look at player
        Vector3 lookDirection = (m_Target.position - transform.position).normalized;
        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void CheckAttackRange()
    {
        if (m_Target == null) return;
        
        float distToPlayer = Vector3.Distance(transform.position, m_Target.position);
        if (distToPlayer < attackRange)
        {
            // TODO: Call damage function
            Debug.Log("Player in attack range!");
        }
    }

    private void BeginFollowing()
    {
        if (!hasStartedFollowing)
        {
            hasStartedFollowing = true;
            if (flappingSoundClip != null)
            {
                _flappingAudioSource = SoundFXManager.instance.PlayLoopingSound(flappingSoundClip, transform, 0.7f);
            }
        }
    }

    public void OnSpawned()
    {
        Debug.Log("OnSpawned called on " + gameObject.name);
        
        if (flyingSoundClip != null)
        {
            SoundFXManager.instance.PlaySoundFX(flyingSoundClip, transform, 0.5f);
        }

        BeginFollowing();
    }



    // Gizmo to visualize attack range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        if (m_Target != null)
        {
            Gizmos.color = Color.blue;
            Vector3 targetPos = m_Target.position - (m_Target.forward * followDistance);
            targetPos.y = m_Target.position.y + flyingHeight;
            Gizmos.DrawWireSphere(targetPos, 0.5f);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, targetPos);
        }
    }
}