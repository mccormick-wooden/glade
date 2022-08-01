using UnityEngine;

public class CrystalSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject crystalPrefab;

    public float minSpawnDistance = 3f;
    public float maxSpawnDistance = 10f;
    public float maxSpawnSlopeDeg = 20f;
    public float maxSpawnHeightDelta = 5f;

    [Tooltip("Min time between spawns")]
    [SerializeField]
    private float minSpawnDelta = 10f;

    // todo; why does this need to be 0?
    [SerializeField]
    public float spawnColliderRadius = 0f; 

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

    private Vector3 ClampToTerrainHeight(Vector3 position)
    {
        Vector3 modifiedPosition = position;
        // clamp to terrain height
        if (null != Terrain.activeTerrain)
            modifiedPosition.y = Terrain.activeTerrain.SampleHeight(position);

        return modifiedPosition;
    }


    private Vector3 generateRandomSpawnPoint()
    {
        // get a random distance within range
        float r = Mathf.Lerp(minSpawnDistance, maxSpawnDistance, Random.value);

        // get a random rotation at that the given distance
        Vector3 spawnPoint = Quaternion.Euler(0, Random.Range(-180, 180), 0) * (r * Vector3.right) + transform.position;

        spawnPoint = ClampToTerrainHeight(spawnPoint);

        return spawnPoint;
    }

    private bool isValidSpawnPoint(Vector3 position)
    {
        bool isValid = true;

        // Check for physics collisions at location
        if (Physics.CheckSphere(position, spawnColliderRadius))
        {
            Debug.Log($"{name}: Attempted to spawn at {position}, but failed due to predicted collision.");
            isValid = false;
        }

        // Check for high sloped terrain
        // TODO: There's probably a better way to do this:
        // Get 2D terrain position
        Terrain terrain = Terrain.activeTerrain;
        Vector3 terrainPos3 = position - terrain.transform.position;
        Vector2 terrainPos = new Vector2(terrainPos3.x, terrainPos3.z);
        // Determine slope at position
        TerrainData terrainData = terrain.terrainData;
        Vector2 terrainSize = new Vector2(terrainData.size.x, terrainData.size.y);
        Vector2 normPos = terrainPos / terrainSize;
        float slope = terrainData.GetSteepness(normPos.x, normPos.y);
        if (slope > maxSpawnSlopeDeg)
        {
            Debug.Log($"{name}: Attempted to spawn at {position}, but failed because slope is {slope}deg (max {maxSpawnSlopeDeg}).");
            isValid = false;
        }

        // Check for height difference
        if (Mathf.Abs(position.y - transform.position.y) > maxSpawnHeightDelta)
        {
            Debug.Log($"{name}: Attempted to spawn at {position}, but failed because height difference is too great.");
            isValid = false;
        }

        return isValid;
    }

    private Vector3 findValidSpawnPoint()
    {
        Vector3 position = generateRandomSpawnPoint();
        if (isValidSpawnPoint(position))
            return position;
        else
            return Vector3.zero;
    }

    public bool SpawnAt(Vector3 spawnPoint)
    {
        spawnPoint = ClampToTerrainHeight(spawnPoint);
        if (!Mathf.Approximately(spawnPoint.magnitude, 0f))
        {
            crystalManager.SpawnCrystal(spawnPoint);
        }
        return true;
    }

    public bool Spawn()
    {
        Vector3 spawnPoint = findValidSpawnPoint();
        if (!Mathf.Approximately(spawnPoint.magnitude, 0f))
        {
            crystalManager.SpawnCrystal(spawnPoint);
            return true;
        }
        else
        {
            return false;
        }
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
                    if (Spawn())
                        timeSinceSpawn = 0;
                }
            }
        }
        else
        {
            timeSinceSpawn = 0;
        }
    }
}
