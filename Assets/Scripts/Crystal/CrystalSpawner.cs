using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beacons;

[RequireComponent(typeof(CrystalController))]
public class CrystalSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject crystalPrefab;

    public float minSpawnDistance = 3f;
    public float maxSpawnDistance = 20f;

    [Tooltip("Min time between spawns")]
    [SerializeField]
    private float minSpawnDelta = 10f;

    [SerializeField]
    public float spawnColliderRadius { get; private set; }

    [Tooltip("Spawn chance per second")]
    [SerializeField]
    private float spawnChancePerSecond = 0.05f;

    private float timeSinceSpawn;
    public bool spawnActive = true;

    private CrystalManager crystalManager;
    
    private void Awake()
    {
        crystalManager = FindObjectOfType<CrystalManager>();
        Utility.LogErrorIfNull(crystalManager, nameof(crystalManager), "Requires crystal manager to spawn crystals.");
    }

    private void OnEnable()
    {
        timeSinceSpawn = 0f;
    }

    // Start is called before the first frame update
    void Start() {}

    private Vector3 generateRandomSpawnPoint()
    {
        // get a random distance within range
        float r = Mathf.Lerp(minSpawnDistance, maxSpawnDistance, Random.value);

        // get a random rotation at that the given distance
        Vector3 spawnPoint = Quaternion.Euler(0, Random.Range(-180, 180), 0) * (r * Vector3.right) + transform.position;

        // clamp to terrain height
        if (null != Terrain.activeTerrain)
            spawnPoint.y = Terrain.activeTerrain.SampleHeight(spawnPoint);

        return spawnPoint;
    }

    private Vector3 findValidSpawnPoint()
    {
        Vector3 position = generateRandomSpawnPoint();
        if (Physics.CheckSphere(position, spawnColliderRadius))
        {
            Debug.Log($"{name}: Attempted to spawn at {position}, but failed due to predicted collision.");
            return Vector3.zero;
        }

        return position;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnActive)
        {
            timeSinceSpawn += Time.deltaTime;
            if (timeSinceSpawn >= minSpawnDelta)
            {
                float spawnValue = Random.value;
                if (spawnValue <= (spawnChancePerSecond * Time.deltaTime))
                {
                    Debug.Log($"{name} attempting spawn.");
                    Vector3 spawnPoint = findValidSpawnPoint();
                    if (!Mathf.Approximately(spawnPoint.magnitude, 0f))
                    {
                        crystalManager.SpawnCrystal(spawnPoint);
                        timeSinceSpawn = 0;
                    }
                }
            }
        }
        else
        {
            timeSinceSpawn = 0;
        }
    }
}
