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
    private BeaconSpawner beaconSpawner;

    [Tooltip("Starting radius around landed beacon to search for a spot to spawn a crystal.")]
    [SerializeField]
    private float startingSearchRadius = 6f;
    [Tooltip("Factor to multiply the search radius with each failed spawn attempt.")]
    [SerializeField]
    private float searchIncreaseFactor = 2f;

    private static float nextCrystalId = 0;

    /// <summary>
    /// SpawnCollider radius to use when checking if new crystal placement is
    /// valid. This is taken from the crystal prefab on Start
    /// </summary>
    private float spawnColliderRadius;

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
    public GameObject FindNearest(Vector3 position)
    {
        float minDistance = -1;
        GameObject nearestCrystal = null;
        foreach (var crystal in crystals)
        {
            if (crystal.Value.isActiveAndEnabled)
            {
                float distance = Vector3.Distance(position, crystal.Value.transform.position);
                if (distance < minDistance || minDistance < 0)
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
        Utility.LogErrorIfNull(beaconSpawner, nameof(beaconSpawner), "beaconSpawner reference is required to spawn crystals from landed beacons.");
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

        // Get the spawnCollider radius from the prefab
        CrystalSpawner spawner = crystalPrefab?.GetComponent<CrystalSpawner>();
        if (spawner != null)
            spawnColliderRadius = spawner.spawnColliderRadius;

        // Subscribe to new beacon landed events to spawn new crystals on landing
        if (null != beaconSpawner)
            beaconSpawner.NewBeaconLanded += OnNewBeaconLanded;
    }

    private Vector3 generateRandomSpawnPoint(Vector3 position, float minRadius, float maxRadius)
    {
        // get a random distance within range
        float r = Mathf.Lerp(minRadius, maxRadius, Random.value);

        // get a random rotation at that the given distance
        Vector3 spawnPoint = Quaternion.Euler(0, Random.Range(-180, 180), 0) * (r * Vector3.right) + position;

        // clamp to terrain height
        if (null != Terrain.activeTerrain)
            spawnPoint.y = Terrain.activeTerrain.SampleHeight(spawnPoint);

        return spawnPoint;
    }

    private void OnNewBeaconLanded(BeaconSpawner beaconSpawner, GameObject beacon)
    {
        float maxNumAttempts = 5;
        float numAttempts = 0;
        bool spawnValid = false;
        float searchRadius = startingSearchRadius;
        while (!spawnValid && (numAttempts < maxNumAttempts))
        {
            Vector3 position = generateRandomSpawnPoint(beacon.transform.position, 2, searchRadius);
            if (Physics.CheckSphere(position, spawnColliderRadius))
            {
                // Log the attempt if there's a collision and double the search radius
                numAttempts++;
                searchRadius *= searchIncreaseFactor;
            }
            else
            {
                spawnValid = true;
                SpawnCrystal(position);
            }
        }

        if (!spawnValid)
            Debug.Log($"Attempt to spawn crystal near {beacon.name} failed after {numAttempts} attempts.");
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
