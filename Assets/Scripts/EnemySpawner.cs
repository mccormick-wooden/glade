using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.AI;
using Beacons;
using Assets.Scripts.Interfaces;


public class EnemySpawner : MonoBehaviour
{
    // Let other things maybe turn this on and off (maybe terrain triggers)
    public bool spawnActive;

    [SerializeField]
    private List<GameObject> beacons;

    [SerializeField]
    private List<GameObject> possibleEnemies;

    [SerializeField]
    private LayerMask whatIsGround;

    private DateTime lastSpawnTime;

    [SerializeField]
    private float nominalSpawnTime;

    [SerializeField]
    private float minTimeBetweenSpawnsSeconds;

    [SerializeField]
    private BeaconSpawner beaconSpawner;

    // Start is called before the first frame update
    void Start()
    {
        spawnActive = true;
        lastSpawnTime = DateTime.Now;
        if (beaconSpawner != null)
            beaconSpawner.NewBeaconLanded += NewBeaconEventHandler;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (DateTime.Now > lastSpawnTime.AddSeconds(minTimeBetweenSpawnsSeconds))
        {
            float weight = CalculateSpawnWeight();
            GenerateEnemies(weight);
        }
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
            totalWeight *= nominalSpawnTime;
        }

        return totalWeight;
    }

    void GenerateEnemies(float weight)
    {
        float r = Random.Range(0.0f, 1.0f);
        GameObject enemiesParent = GameObject.Find("EnemiesParent");

        if (weight > r)
        {
            Debug.Log("Make a bad guy!");

            Vector3 newPosition;
            bool FoundValidPosition = FindValidPlacement(transform.position, out newPosition);

            if (!FoundValidPosition)
                return;

            int randomEnemy = Random.Range(0, possibleEnemies.Count);

            GameObject enemyPrefab = possibleEnemies[randomEnemy];
            GameObject g = Instantiate(enemyPrefab, new Vector3(0,0,0), Quaternion.identity, enemiesParent.transform);
            
            g.GetComponent<NavMeshAgent>().Warp(newPosition);
            g.GetComponent<NavMeshAgent>().enabled = true;

            lastSpawnTime = DateTime.Now;
        }
    }

    bool FindValidPlacement(Vector3 position, out Vector3 newPosition)
    {
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

            Vector3 samplePosition = new Vector3(randomX, 0, randomZ) + transform.position;

            NavMeshHit hit;
            bool gotHit = NavMesh.SamplePosition(samplePosition, out hit, 2, NavMesh.AllAreas);

            if (!gotHit)
            {
                attempts++;
                continue;
            }


            //g.transform.position = hit.position;
            newPosition = hit.position;
            return true;
        }

        newPosition = Vector3.zero;
        return false;
    }

    public void NewBeaconEventHandler(Beacons.BeaconSpawner beaconSpawner, GameObject crashedBeacon)
    {
        beacons.Add(crashedBeacon);
        IDamageable beaconDamageModel = crashedBeacon.GetComponent<IDamageable>();
        if (beaconDamageModel != null)
            beaconDamageModel.Died += BeaconDeathEventHandler;
    }

    public void BeaconDeathEventHandler(IDamageable damageModel, string name, int instanceId)
    {
        damageModel.Died -= BeaconDeathEventHandler;
        GameObject deadBeacon = beacons.Find(delegate (GameObject beacon)
           {
               return beacon.GetInstanceID() == instanceId;
           }
        );

        beacons.Remove(deadBeacon);
    }
}
