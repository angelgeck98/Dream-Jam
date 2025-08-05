using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Spawner : MonoBehaviour
{
    public GameObject birdPrefab;

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 randomSpawnPosition = new Vector3(Random.Range(-10,11), 5, Random.Range(-10,11));   
            
            GameObject newBird = Instantiate(birdPrefab, randomSpawnPosition, Quaternion.identity);


            
            FlyingAI birdAI = newBird.GetComponent<FlyingAI>();
            
            if (birdAI != null)
            {
                birdAI.OnSpawned();
            }
        }
    }
}
