using System;
using System.Collections;
using Assets.Scripts.Interfaces;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using DigitalRuby.PyroParticles;

namespace Beacons
{
    public class BeaconSpawner : MonoBehaviour
    {
        public static BeaconSpawner Instance { get; private set; }

        /// <summary>
        /// Event that LevelStateManagers should subscribe to in order to be notified
        /// that all beacons owned by this spawner have been killed.
        /// </summary>
        public Action AllBeaconsDied { get; set; }

        public Action BeaconDied { get; set; }

        /// <summary>
        /// Event indicating a new beacon has landed. Provides a reference to the landed beacon GameObject.
        /// </summary>
        public Action<BeaconSpawner, GameObject> NewBeaconLanded { get; set; } 

        /// <summary>
        /// The type of beacon that will be spawned by this spawner.
        /// </summary>
        [Header("Config Settings")] [SerializeField]
        public GameObject beaconPrefab;

        public GameObject fallingBeaconPrefab;

        /// <summary>
        /// The time in seconds that will elapse before the first beacon is spawned
        /// </summary>
        [SerializeField] private float initialBeaconSpawnTime;

        [SerializeField] private float minSpawnWaitTime;

        [SerializeField] private float maxSpawnWaitTime;

        /// <summary>
        /// The maximum number of beacons that will be active at a given time.
        /// The spawner compares currentBeaconCount to maxConcurrentBeacons to determine if a beacon should be spawned.
        /// </summary>
        [SerializeField] private float maxConcurrentBeacons;

        /// <summary>
        /// The maximum number of beacons that will be spawned by the spawner as calculated by the amount of available spawn points.
        /// </summary>
        private float maxTotalBeacons;

        /// <summary>
        /// Provide a TMPro GUI object to display the current beacon count.
        /// </summary>
        [SerializeField] private TextMeshProUGUI beaconCountText;

        /// <summary>
        /// A runtime record of how many beacons owned by this spawner currently exist.
        /// </summary>
        [Header("Runtime Values")] [SerializeField]
        private int currentBeaconCount;

        /// <summary>
        /// A runtime record of how many beacons owned by this spawner have been spawned.
        /// </summary>
        [SerializeField] private int totalBeaconsSpawnedCount;

        /// <summary>
        /// A runtime record of how many beacons owned by this spawner have been destroyed.
        /// </summary>
        [SerializeField] private int totalBeaconsDiedCount;

        /// <summary>
        /// A runtime record of how much time is remaining until the next beacon is spawned
        /// </summary>
        [SerializeField] private float spawnTimerRemaining;

        private List<BeaconSpawnPoint> beaconSpawnPoints;

        private void Awake()
        {
            // Enforced Singleton
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            spawnTimerRemaining = initialBeaconSpawnTime;
        }

        private void Start()
        {
            beaconSpawnPoints = new List<BeaconSpawnPoint>(FindObjectsOfType<BeaconSpawnPoint>());
            beaconSpawnPoints.Sort((a, b) => a.spawnOrder.CompareTo(b.spawnOrder));
            maxTotalBeacons = beaconSpawnPoints.Count;
        }

        private void Update()
        {
            spawnTimerRemaining -= Time.deltaTime;

            if (spawnTimerRemaining <= 0)
            {
                Spawn();
                spawnTimerRemaining = Random.Range(minSpawnWaitTime, maxSpawnWaitTime);
            }
        }

        public void Spawn()
        {
            if ((currentBeaconCount == maxConcurrentBeacons) || (totalBeaconsSpawnedCount == maxTotalBeacons))
                return;

            // When spawning, we take the next spawn-to location and Instantiate the crashed beacon there
            var beaconSpawnPoint = beaconSpawnPoints[0];
            beaconSpawnPoints.RemoveAt(0);

            // Instantiate the falling beacon prefab and listen for the collision delegate
            var transformRef = beaconSpawnPoint.transform;
            var transformPositionRef = transformRef.position;
            var fallingBeaconAdjustedPosition = new Vector3(transformPositionRef.x, transformPositionRef.y + 100,
                transformPositionRef.z);

            var fallingBeaconRotation = new Vector3(90, 0, 0);
            var fallingBeacon = Instantiate(fallingBeaconPrefab, fallingBeaconAdjustedPosition,
                Quaternion.Euler(fallingBeaconRotation),
                transform)?.GetComponent<FireProjectileScript>();

            Utility.LogErrorIfNull(fallingBeacon, "fallingBeacon");

            if (fallingBeacon != null)
            {
                fallingBeacon.CollisionDelegate += OnBeaconLanded;
            }
        }

        private void OnBeaconLanded(FireProjectileScript script, Vector3 landingSpot)
        {
            script.CollisionDelegate -= OnBeaconLanded;

            var crashedBeacon = Instantiate(beaconPrefab, landingSpot, Quaternion.identity, transform)
                ?.GetComponent<CrashedBeacon>();

            if (crashedBeacon == null)
            {
                Utility.LogErrorIfNull(crashedBeacon, nameof(crashedBeacon));
                return;
            }

            OnBeaconReadyForDamage(crashedBeacon.gameObject);
            IncrementBeaconCount();
            Debug.Log("Spawned new beacon!");
        }

        private void OnBeaconReadyForDamage(GameObject beacon)
        {
            NewBeaconLanded?.Invoke(this, beacon);

            var beaconDamageModel = beacon.GetComponent<IDamageable>();
            beaconDamageModel.Died += OnBeaconDeath;
        }

        public void OnBeaconDeath(IDamageable damageModel, string name, int instanceId)
        {
            BeaconDied?.Invoke();

            damageModel.Died -= OnBeaconDeath;

            totalBeaconsDiedCount++;
            DecrementBeaconCount();

            if (totalBeaconsDiedCount == maxTotalBeacons)
            {
                AllBeaconsDied?.Invoke();
                enabled = false;
            }
            else
            {
                Spawn();
            }
        }

        private void DecrementBeaconCount()
        {
            currentBeaconCount--;
            SetBeaconCountText();
        }

        private void IncrementBeaconCount()
        {
            currentBeaconCount++;
            totalBeaconsSpawnedCount++;
            SetBeaconCountText();
        }

        private void SetBeaconCountText()
        {
            if (beaconCountText == null)
                return;
            beaconCountText.text = $"Beacons: {currentBeaconCount}";
        }
    }
}
