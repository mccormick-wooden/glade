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

    [SerializeField]
    private GameObject enemySpawnParticlePrefab;

    public Action<IDamageable> EnemySpawned;

    const int MELEE_ENEMY_TYPE = 0;
    const int RANGED_ENEMY_TYPE = 1;
    const int AOE_ENEMY_TYPE = 2;

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

            float r = Random.Range(0.0f, 1.0f);
            if (weight > r)
            {
                GenerateRandomEnemy();
            }
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

    void GenerateRandomEnemy()
    {
        int randomEnemy = Random.Range(0, possibleEnemies.Count);

        GameObject enemyPrefab = possibleEnemies[randomEnemy];
        GenerateEnemy(enemyPrefab, transform.position);
    }

    void GenerateEnemy(GameObject enemyPrefab, Vector3 location)
    {
        GameObject enemiesParent = GameObject.Find("EnemiesParent");

        Vector3 newPosition;
        bool FoundValidPosition = FindValidPlacement(location, out newPosition);

        if (!FoundValidPosition)
            return;

        GameObject g = Instantiate(enemyPrefab, new Vector3(0, 0, 0), Quaternion.identity, enemiesParent.transform);

        g.GetComponent<NavMeshAgent>().Warp(newPosition);
        g.GetComponent<NavMeshAgent>().enabled = true;

        GameObject particles = Instantiate(enemySpawnParticlePrefab, new Vector3(0, 0, 0), Quaternion.identity, enemiesParent.transform);
        particles.transform.position = newPosition;

        lastSpawnTime = DateTime.Now;

        EnemySpawned?.Invoke(g.GetComponent<IDamageable>());
    }

    bool FindValidPlacement(Vector3 position, out Vector3 newPosition)
    {
        int attempts = 0;
        const int maxAttempts = 5;

        while (attempts < maxAttempts)
        {
            float randomDistance = Random.Range(10f, 20f);
            float randomAngle = Random.Range(0f, 359f);

            float randomX = (float)Math.Cos(randomAngle) * randomDistance;
            float randomZ = (float)Math.Sin(randomAngle) * randomDistance;

            Vector3 samplePosition = new Vector3(randomX, 1, randomZ) + position;

            NavMeshHit hit;
            bool gotHit = NavMesh.SamplePosition(samplePosition, out hit, 2, NavMesh.AllAreas);

            if (!gotHit)
            {
                attempts++;
                continue;
            }

            newPosition = hit.position;
            return true;
        }

        newPosition = Vector3.zero;
        return false;
    }

    IEnumerator GenerateBeaconEnemies(GameObject crashedBeacon)
    {
        const int numberOfMeleeEnemies = 2;
        const int numberOfRangedEnemies = 2;
        const int numberOfAOEEnemies = 1;
        

        for (int i = 0; i < numberOfMeleeEnemies; i++)
        {
            GenerateEnemy(possibleEnemies[MELEE_ENEMY_TYPE], crashedBeacon.transform.position);

            yield return new WaitForSeconds(Random.Range(0.25f, 0.5f));
        }

        for (int i = 0; i < numberOfRangedEnemies; i++)
        {
            GenerateEnemy(possibleEnemies[RANGED_ENEMY_TYPE], crashedBeacon.transform.position);

            yield return new WaitForSeconds(Random.Range(0.25f, 0.5f));
        }

        for (int i = 0; i < numberOfAOEEnemies; i++)
        {
            GenerateEnemy(possibleEnemies[AOE_ENEMY_TYPE], crashedBeacon.transform.position);
            yield return new WaitForSeconds(Random.Range(0.25f, 0.5f));
        }

    }

    public void NewBeaconEventHandler(Beacons.BeaconSpawner beaconSpawner, GameObject crashedBeacon)
    {
        beacons.Add(crashedBeacon);
        IDamageable beaconDamageModel = crashedBeacon.GetComponent<IDamageable>();
        if (beaconDamageModel != null)
            beaconDamageModel.Died += BeaconDeathEventHandler;

        // spawn some enemies directly from the beacon
        // temp variables for now, maybe make this random later?

        StartCoroutine(GenerateBeaconEnemies(crashedBeacon));
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
