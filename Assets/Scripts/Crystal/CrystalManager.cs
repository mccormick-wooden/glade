using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beacons;
using Assets.Scripts.Interfaces;

public class CrystalManager : MonoBehaviour
{

    [SerializeField]
    private GameObject crystalPrefab;
    [SerializeField]
    private GameObject crystalSpawnerPrefab;

    [SerializeField]
    private BeaconSpawner beaconSpawner;

    [SerializeField]
    private GameObject[] CrystalSpawnPoints;

    [Tooltip("Max attempts to spawn crystal at crashed beacon before giving up.")]
    [SerializeField]
    private int maxSpawnAttempts = 5;

    private int railsSpawnId = 0;

    private static float nextCrystalId = 0;
    private CrystalSpawner spawner;

    /// <summary>
    /// List of all crystals instantiated in the game 
    /// </summary>
    private Dictionary<float, CrystalController> crystals = new Dictionary<float, CrystalController>();

    /// <summary>
    /// Gets a reference to the nearest crystal
    /// </summary>
    /// <param name="position">
    /// Position to search for nearest crystal
    /// </param>
    /// <returns>
    /// GameObject reference to crystal nearest to provided position.
    /// Null if no crystals exist that are active and enabled.
    /// </returns>
    public GameObject FindNearestCrystal(Vector3 position)
    {
        float minDistance = float.MaxValue;
        GameObject nearestCrystal = null;
        foreach (var crystal in crystals)
        {
            if (crystal.Value.isActiveAndEnabled)
            {
                float distance = Vector3.Distance(position, crystal.Value.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestCrystal = crystal.Value.gameObject;
                }
            }
        }

        return nearestCrystal;
    }

    private void Awake()
    {
        if (null == beaconSpawner)
            Debug.LogWarning($"{name}: beaconSpawner reference is required to spawn crystals from landed beacons.");

        Utility.LogErrorIfNull(crystalPrefab, nameof(crystalPrefab), "crystalPrefab reference is required.");
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get a list of existing instantiated Crystals on start 
        foreach (var crystal in GameObject.FindObjectsOfType<CrystalController>()) {
            crystals[crystal.CrystalID] = crystal;
            Debug.Log($"{name}: Found crystal {crystal.name} ({crystal.GetInstanceID()}) at position {crystal.transform.position}");
        }

        // Subscribe to new beacon landed events to spawn new crystals on landing
        if (null != beaconSpawner)
            beaconSpawner.NewBeaconLanded += OnNewBeaconLanded;
    }

    private void OnNewBeaconLanded(BeaconSpawner beaconSpawner, GameObject beacon)
    {
        spawner = Instantiate(crystalSpawnerPrefab, beacon.transform.position, beacon.transform.rotation, beacon.transform).GetComponent<CrystalSpawner>();
        spawner.name = "BeaconCrystalSpawner";
        spawner.spawnActive = true; //YES WE DO --> We don't want this continually spawning crystals
        int numAttempts = 0;

        Vector3 spawnPoint = CrystalSpawnPoints[railsSpawnId].transform.position;
        Debug.Log($"Spawning at Crystal Spawn Point {CrystalSpawnPoints[railsSpawnId].name}: {spawnPoint}");
        bool spawned = spawner.SpawnAt(spawnPoint);
        railsSpawnId++;

        if(railsSpawnId >= CrystalSpawnPoints.Length)
            railsSpawnId = 0;

        if (!spawned)
            Debug.Log($"Attempt to spawn crystal near {beacon.name} failed. after {numAttempts} attempts.");
    }

    private void removeCrystal(IDamageable damageable, string name, int instanceId)
    {
        damageable.Died -= removeCrystal;
        crystals.Remove(instanceId);
    }

    public void SpawnCrystal(Vector3 position)
    {
        // randomize the crystal rotation and instantiate at location
        Vector3 rotation = new Vector3(0f, 360*(Random.value - 0.5f), 0f);
        GameObject crystal = Instantiate(crystalPrefab, position, Quaternion.Euler(rotation), transform);
        crystal.name = $"CrystalSpawn{nextCrystalId++}";

        Debug.Log($"{name} spawning at {position}.");

        // add to list of crystals and subscribe to its death event to remove it
        crystals.Add(crystal.GetInstanceID(), crystal.GetComponent<CrystalController>());
        crystal.GetComponentInChildren<IDamageable>().Died += removeCrystal;
    }


}
