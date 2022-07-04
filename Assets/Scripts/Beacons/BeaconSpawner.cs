using System;
using Assets.Scripts.Interfaces;
using DigitalRuby.PyroParticles;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

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

        /// <summary>
        /// Event indicating a new beacon has landed. Provides a reference to the landed beacon GameObject.
        /// </summary>
        public Action<BeaconSpawner, GameObject> NewBeaconLanded { get; set; } 

        /// <summary>
        /// The type of beacon that will be spawned by this spawner.
        /// </summary>
        [Header("Config Settings")]
        [SerializeField]
        public GameObject beaconToSpawn;

        /// <summary>
        /// The time in seconds that will elapse before the first beacon is spawned
        /// </summary>
        [SerializeField] 
        private float initialBeaconSpawnTime;

        [SerializeField]
        private float minSpawnWaitTime;

        [SerializeField]
        private float maxSpawnWaitTime;

        /// <summary>
        /// The maximum number of beacons that will be active at a given time.
        /// The spawner compares currentBeaconCount to maxConcurrentBeacons to determine if a beacon should be spawned.
        /// </summary>
        [SerializeField]
        private float maxConcurrentBeacons;

        /// <summary>
        /// The maximum number of beacons that will be spawned by the spawner.
        /// </summary>
        [SerializeField]
        private float maxTotalBeacons;

        /// <summary>
        /// Provide a TMPro GUI object to display the current beacon count.
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI beaconCountText;

        /// <summary>
        /// A runtime record of how many beacons owned by this spawner currently exist.
        /// </summary>
        [Header("Runtime Values")]
        [SerializeField]
        private int currentBeaconCount;

        /// <summary>
        /// A runtime record of how many beacons owned by this spawner have been spawned.
        /// </summary>
        [SerializeField]
        private int totalBeaconsSpawnedCount;

        /// <summary>
        /// A runtime record of how many beacons owned by this spawner have been destroyed.
        /// </summary>
        [SerializeField]
        private int totalBeaconsDiedCount;

        /// <summary>
        /// A runtime record of how much time is remaining until the next beacon is spawned
        /// </summary>
        [SerializeField]
        private float spawnTimerRemaining;

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

            var transformRef = transform;
            var beaconManager = Instantiate(beaconToSpawn, transformRef.position, transformRef.rotation, transform)
                ?.GetComponent<BeaconManager>();

            if (beaconManager == null)
                Utility.LogErrorIfNull(beaconManager, nameof(beaconManager));

            beaconManager.BeaconReadyForDamage += OnBeaconReadyForDamage;

            IncrementBeaconCount();
            
            Debug.Log("Spawned new beacon!");
        }

        private void OnBeaconReadyForDamage(BeaconManager beaconManager, GameObject beacon)
        {
            beaconManager.BeaconReadyForDamage -= OnBeaconReadyForDamage;
            NewBeaconLanded?.Invoke(this, beacon);

            var beaconDamageModel = beacon.GetComponent<IDamageable>();
            beaconDamageModel.Died += OnBeaconDeath;
        }

        public void OnBeaconDeath(IDamageable damageModel, string name, int instanceId)
        {
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
