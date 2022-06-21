using System;
using UnityEngine;
using TMPro;
using Unity.Collections;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Beacons
{
    public class BeaconSpawner : MonoBehaviour
    {
        public static BeaconSpawner Instance { get; private set; }

        [SerializeField]
        public GameObject beaconToSpawn;

        // The original value for this variable controls when the first beacon will drop.
        [SerializeField]
        private float spawnTimerRemaining;

        [SerializeField]
        private float minSpawnWaitTime;

        [SerializeField]
        private float maxSpawnWaitTime;

        [SerializeField]
        private int beaconCount;

        [SerializeField]
        private int beaconTotal;

        [SerializeField]
        private float maxConcurrentBeacons;

        [SerializeField]
        private float maxTotalBeacons;

        // Provide a TMPro GUI object to output the current count
        [SerializeField]
        private TextMeshProUGUI beaconCountText;

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
            if ((beaconCount >= maxConcurrentBeacons) || (beaconTotal >= maxTotalBeacons))
                return;

            var transformRef = transform;
            IncrementBeaconCount();
            Instantiate(beaconToSpawn, transformRef.position, transformRef.rotation, transform);
        }

        public void OnBeaconDeath(CrashedBeacon deadBeacon)
        {
            Destroy(deadBeacon);
            DecrementBeaconCount();
            Spawn();
        }

        private void DecrementBeaconCount()
        {
            SetBeaconCountText(false);
        }

        private void IncrementBeaconCount()
        {
            IncrementBeaconTotal();
            SetBeaconCountText(true);
        }

        private void IncrementBeaconTotal()
        {
            beaconTotal++;
        }

        private void SetBeaconCountText(bool increment)
        {
            var oldCount = beaconCount;
            beaconCount = increment ? ++oldCount : --oldCount;
            if (beaconCountText == null)
                return;
            beaconCountText.text = $"Beacons: {(beaconCount >= 0 ? beaconCount : 0)}";
        }
    }
}
