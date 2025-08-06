using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject birdPrefab;
    
    [Header("Spawn Range")]
    [SerializeField] private float spawnRangeX = 10f;
    [SerializeField] private float spawnRangeZ = 10f;
    [SerializeField] private float spawnHeight = 5f;
    
    [Header("Gizmo Settings")]
    [SerializeField] private Color gizmoColor = Color.green;
    [SerializeField] private bool showSpawnArea = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 randomSpawnPosition = new Vector3(
                Random.Range(-spawnRangeX, spawnRangeX), 
                spawnHeight, 
                Random.Range(-spawnRangeZ, spawnRangeZ)
            );   
            
            GameObject newBird = Instantiate(birdPrefab, randomSpawnPosition, Quaternion.identity);
            
            FlyingAI birdAI = newBird.GetComponent<FlyingAI>();
            
            if (birdAI != null)
            {
                birdAI.OnSpawned();
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        if (!showSpawnArea) return;
        
        Gizmos.color = gizmoColor;
        
        // Draw wireframe cube showing spawn area
        Vector3 center = transform.position + new Vector3(0, spawnHeight, 0);
        Vector3 size = new Vector3(spawnRangeX * 2, 0.1f, spawnRangeZ * 2);
        
        Gizmos.DrawWireCube(center, size);
        
        // Draw corner markers
        Gizmos.color = Color.red;
        float markerSize = 0.5f;
        
        Vector3[] corners = {
            center + new Vector3(-spawnRangeX, 0, -spawnRangeZ),
            center + new Vector3(spawnRangeX, 0, -spawnRangeZ),
            center + new Vector3(-spawnRangeX, 0, spawnRangeZ),
            center + new Vector3(spawnRangeX, 0, spawnRangeZ)
        };
        
        foreach (Vector3 corner in corners)
        {
            Gizmos.DrawWireSphere(corner, markerSize);
        }
        
        // Draw center point
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, markerSize * 0.7f);
    }
    
    private void OnDrawGizmosSelected()
    {
        if (!showSpawnArea) return;
        
        // Draw a more detailed view when selected
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.3f);
        
        Vector3 center = transform.position + new Vector3(0, spawnHeight, 0);
        Vector3 size = new Vector3(spawnRangeX * 2, 1f, spawnRangeZ * 2);
        
        Gizmos.DrawCube(center, size);
    }
}