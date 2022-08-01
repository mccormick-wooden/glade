using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.AI;
using Beacons;
using Assets.Scripts.Interfaces;
using UnityEngine.Serialization;


public class EnemySpawner : MonoBehaviour
{
    #region Possible Enemy Parameters

    [Header("Enemy Prefab Parameters")]
    /*
     * The list of possible enemies is assumed to look like this:
     * - Flower Fairy  - (infantry)
     * - Plant Dionaea - (infantry)
     * - Frightfly     - (flying infantry)
     * - Mushroom      - (aoe)
     * - Pea Shooter   - (ranged)
     * - Plant Venus   - (heavy)
     */
    [SerializeField]
    private List<GameObject> infantryEnemyPrefabs;

    [SerializeField] private GameObject aoeEnemyPrefab;
    [SerializeField] private GameObject rangedEnemyPrefab;
    [SerializeField] private GameObject heavyEnemyPrefab;

    [SerializeField]
    private GameObject enemySpawnParticlePrefab;

    [FormerlySerializedAs("possibleEnemies")] [SerializeField]
    private List<GameObject> allEnemyPrefabs;

    #endregion

    #region Spawn Parameters

    [Header("Enemy Spawn Parameters")] [SerializeField]
    private float minimumTimeBetweenSpawns;

    [SerializeField] private float maximumTimeBetweenSpawns;

    [SerializeField] private float beaconEnemyDetectionRange;

    [SerializeField] private uint startingNumberOfEnemiesPerBeacon;
    [SerializeField] private uint maximumNumberOfEnemiesPerBeacon;

    [SerializeField] private float startingRatioOfInfantryEnemies;
    [SerializeField] private float startingRatioOfRangedEnemies;
    [SerializeField] private float startingRatioOfAoeEnemies;
    [SerializeField] private float startingRatioOfHeavyEnemies;

    // When the ratio of enemies:max enemies is lower than this we spawn heavy enemies
    [SerializeField] private float criticalEnemyPercentage;

    #endregion

    [SerializeField] private List<GameObject> beacons;

    [SerializeField] private BeaconSpawner beaconSpawner;

    public Action<IDamageable> EnemySpawned;
    
    void Start()
    {
        if (beaconSpawner != null)
            beaconSpawner.NewBeaconLanded += NewBeaconEventHandler;
    }

    public uint GetMaximumNumberOfEnemiesPerBeacon()
    {
        return maximumNumberOfEnemiesPerBeacon;
    }

    private GameObject GetRandomEnemyPrefabFromList(List<GameObject> enemyPrefabs)
    {
        var randomEnemy = Random.Range(0, enemyPrefabs.Count);

        return enemyPrefabs[randomEnemy];
    }

    private GameObject GetRandomEnemyInfantryPrefab()
    {
        return GetRandomEnemyPrefabFromList(infantryEnemyPrefabs);
    }

    // Instantiates an Enemy Prefab in a given location
    private void GenerateEnemy(GameObject enemyPrefab, Vector3 location)
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

        EnemySpawned?.Invoke(g.GetComponent<IDamageable>());
    }

    // Finds a precise position around a given position where an enemy can be spawned
    private bool FindValidPlacement(Vector3 position, out Vector3 newPosition)
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

    // Adds a spawn co-routine for a Beacon anytime a new one appears
    public void NewBeaconEventHandler(BeaconSpawner beaconSpawner, GameObject crashedBeacon)
    {
        beacons.Add(crashedBeacon);
        var beaconDamageModel = crashedBeacon.GetComponent<IDamageable>();
        if (beaconDamageModel != null)
        {
            beaconDamageModel.Died += BeaconDeathEventHandler;
        }

        StartCoroutine(GenerateBeaconEnemies(crashedBeacon));
    }

    // Co-Routine run per Beacon that manages spawning enemies around it
    IEnumerator GenerateBeaconEnemies(GameObject crashedBeacon)
    {
        /* Start by spawning a lump sum of enemies */
        var numberOfInfantryEnemiesToSpawn =
            Mathf.Ceil(startingRatioOfInfantryEnemies * startingNumberOfEnemiesPerBeacon);
        var numberOfAoeEnemiesToSpawn =
            Mathf.Ceil(startingRatioOfAoeEnemies * startingNumberOfEnemiesPerBeacon);
        var numberOfRangedEnemiesToSpawn =
            Mathf.Ceil(startingRatioOfRangedEnemies * startingNumberOfEnemiesPerBeacon);
        var numberOfHeavyEnemiesToSpawn =
            Mathf.FloorToInt(startingRatioOfHeavyEnemies * startingNumberOfEnemiesPerBeacon);

        var spawnPosition = crashedBeacon.transform.position;

        uint numberOfStartingEnemiesSpawned = 0;
        for (var i = 0;
             i < numberOfInfantryEnemiesToSpawn && numberOfStartingEnemiesSpawned < maximumNumberOfEnemiesPerBeacon;
             i++)
        {
            var infantryPrefab = GetRandomEnemyInfantryPrefab();
            GenerateEnemy(infantryPrefab, spawnPosition);
            numberOfStartingEnemiesSpawned++;
        }

        for (var i = 0;
             i < numberOfAoeEnemiesToSpawn && numberOfStartingEnemiesSpawned < maximumNumberOfEnemiesPerBeacon;
             i++)
        {
            GenerateEnemy(aoeEnemyPrefab, spawnPosition);
            numberOfStartingEnemiesSpawned++;
        }

        for (var i = 0;
             i < numberOfRangedEnemiesToSpawn && numberOfStartingEnemiesSpawned < maximumNumberOfEnemiesPerBeacon;
             i++)
        {
            GenerateEnemy(rangedEnemyPrefab, spawnPosition);
            numberOfStartingEnemiesSpawned++;
        }

        for (var i = 0;
             i < numberOfHeavyEnemiesToSpawn && numberOfStartingEnemiesSpawned < maximumNumberOfEnemiesPerBeacon;
             i++)
        {
            GenerateEnemy(heavyEnemyPrefab, spawnPosition);
            numberOfStartingEnemiesSpawned++;
        }

        var beaconDamageable = crashedBeacon.GetComponent<IDamageable>();
        if (beaconDamageable == null || beaconDamageable.IsDead)
        {
            yield break;
        }

        /* Spawn enemies as long as the beacon lives, spawn heavy enemies if the number of enemies is critically low */
        while (!beaconDamageable.IsDead)
        {
            // Check for the number of enemies around the beacon
            var numberOfEnemies = Physics
                .OverlapSphere(spawnPosition, beaconEnemyDetectionRange)
                .Aggregate(
                    0,
                    (acc, collider) => collider.gameObject.GetComponent<BaseEnemy>() == null ? acc : ++acc
                );

            if (numberOfEnemies < maximumNumberOfEnemiesPerBeacon)
            {
                var ratioOfLivingEnemies = numberOfEnemies / maximumNumberOfEnemiesPerBeacon;
                if (ratioOfLivingEnemies < criticalEnemyPercentage)
                {
                    GenerateEnemy(heavyEnemyPrefab, spawnPosition);
                }
                else
                {
                    var randomEnemyPrefab = GetRandomEnemyPrefabFromList(allEnemyPrefabs);
                    GenerateEnemy(randomEnemyPrefab, spawnPosition);
                }
            }

            var spawnWaitTime = Random.Range(minimumTimeBetweenSpawns, maximumTimeBetweenSpawns);
            yield return new WaitForSeconds(spawnWaitTime);
        }
    }

    // Unregister the callback when a beacon dies
    public void BeaconDeathEventHandler(IDamageable damageModel, string _name, int instanceId)
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
