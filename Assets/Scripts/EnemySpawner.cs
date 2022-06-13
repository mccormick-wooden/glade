using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    // Let other things maybe turn this on and off (maybe terrain triggers)
    public bool spawnActive;

    [SerializeField]
    private List<GameObject> beacons;

    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField]
    private LayerMask whatIsGround;

    private DateTime lastSpawnTime;
    public float nominalSpawnTime;

    // Start is called before the first frame update
    void Start()
    {
        spawnActive = true;
        lastSpawnTime = DateTime.Now;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        float weight = CalculateSpawnWeight();
        GenerateEnemies(weight);
    }

    float CalculateSpawnWeight()
    {
        float totalWeight = 0;

        for (int i = 0; i < beacons.Count; i++)
        {
            // make weight fall off as a square of distance?
            // this will probably ramp things way too high too fast,
            // we can adjust later
            float distance = Vector3.Distance(beacons[i].transform.position, transform.position);
            float weight = 1f / (distance * distance) * (float)(DateTime.Now - lastSpawnTime).TotalSeconds;
            totalWeight += weight;
        }

        //Debug.Log(totalWeight);
        return totalWeight;
    }

    void GenerateEnemies(float weight)
    {
        float r = Random.Range(0.0f, 1.0f);
        GameObject enemiesParent = GameObject.Find("ParentEnemy");

        if (weight > r)
        {
            Debug.Log("Make a bad guy!");

            GameObject g = Instantiate(enemyPrefab, enemiesParent.transform);

            if (!FindValidPlacement(g))
            {
                Destroy(g);
            }
            else
            {
                lastSpawnTime = DateTime.Now;
            }
        }
    }

    bool FindValidPlacement(GameObject g)
    {
        // Move enemy to a random location that isn't inside anything
        g.transform.position = transform.position;

        const float MIN_X_OFFSET = 10;
        const float MAX_X_OFFSET = 25;

        const float MIN_Z_OFFSET = 10;
        const float MAX_Z_OFFSET = 25;

        int attempts = 0;
        const int maxAttempts = 3;

        while (attempts < maxAttempts)
        {
            float randomX = UnityEngine.Random.Range(MIN_X_OFFSET, MAX_X_OFFSET) * ((UnityEngine.Random.Range(0f, 1f) >= 0.5) ? -1 : 1);
            float randomZ = UnityEngine.Random.Range(MIN_Z_OFFSET, MAX_Z_OFFSET) * ((UnityEngine.Random.Range(0f, 1f) >= 0.5) ? -1 : 1);

            g.transform.position += new Vector3(randomX, 0, randomZ);

            // move it up until we get to the ground 
            float halfHeight = g.GetComponent<BaseEnemy>().Height / 2f;
            g.transform.position += new Vector3(0, halfHeight, 0);

            float groundInterceptRayLength = halfHeight + 0.1f;

            int yPositionAttempts = 0;
            const int maxYPositionAttempts = 20;
            bool gotValidYPosition = false;

            // Try going up from our current position first
            for (yPositionAttempts = 0; yPositionAttempts < maxYPositionAttempts; yPositionAttempts++)
            {
                if (Physics.Raycast(g.transform.position, Vector3.down, groundInterceptRayLength, whatIsGround))
                {

                    gotValidYPosition = true;
                    break;
                }

                g.transform.position += new Vector3(0, 0.1f, 0);
            }

            if (gotValidYPosition)
            {
                return true;
            }

            // If we couldn't figure it out going up, try down
            g.transform.position = transform.position + new Vector3(randomX, 0, randomZ);

            // Then try going down
            for (yPositionAttempts = 0; yPositionAttempts < maxYPositionAttempts; yPositionAttempts++)
            {
                if (Physics.Raycast(g.transform.position, Vector3.down, groundInterceptRayLength, whatIsGround))
                {
                    gotValidYPosition = true;
                    break;
                }

                g.transform.position -= new Vector3(0, 0.1f, 0);
            }

            // if we still didn't get anything -
            if (!gotValidYPosition)
            {
                // failed to find a valid placement, 
                // give up and start again
                attempts++;
            }
            else
            {
                return true;
            }
        }

        return false;
    }
}
