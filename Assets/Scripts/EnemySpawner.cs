using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public bool spawnActive;

    public List<GameObject> beacons;

    public List<GameObject> possibleEnemies;

    [SerializeField]
    public LayerMask whatIsGround;

    DateTime lastSpawnTime;
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
        float r = UnityEngine.Random.Range(0.0f, 1.0f);
        GameObject enemiesParent = GameObject.Find("ParentEnemy");

        if (weight > r)
        {
            Debug.Log("Make a bad guy!");

            
            GameObject enemyPrefab = possibleEnemies[1];
            GameObject g = Instantiate(enemyPrefab, enemiesParent.transform);

            //UnityEditor.PrefabUtility.SaveAsPrefabAsset(g, possibleEnemies[0]);

            //g.transform.parent = enemiesParent.transform;

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

        bool safeTransform = false;

        const float minimumXOffset = 10;
        const float maximumXOffset = 25;

        const float minimumZOffset = 10;
        const float maximumZOffset = 25;

        int attempts = 0;
        const int maxAttempts = 3;

        while (safeTransform == false && attempts < maxAttempts)
        {
            float randomX = UnityEngine.Random.Range(minimumXOffset, maximumXOffset) * ((UnityEngine.Random.Range(0f, 1f) >= 0.5) ? -1 : 1);
            float randomZ = UnityEngine.Random.Range(minimumZOffset, maximumZOffset) * ((UnityEngine.Random.Range(0f, 1f) >= 0.5) ? -1 : 1);

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

            if (gotValidYPosition == true)
            {
                safeTransform = true;
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
            if (gotValidYPosition == false)
            {
                // failed to find a valid placement, 
                // give up and start again
                attempts++;
            }
            else
            {
                safeTransform = true;
                //g.transform.position = newPosition;
                return true;
            }
        }

        return false;
    }
}
