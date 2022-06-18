using System;
using UnityEngine;
using TMPro;
using Unity.Collections;

namespace Beacons
{
    
    public class BeaconSpawner : MonoBehaviour
    {
        [HideInInspector]
        public static BeaconSpawner Instance
        {
            get;
            private set;
        }
        
        [SerializeField] public GameObject beacon;

        [SerializeField]
        private float spawnTimer;

        [SerializeField] private float maxConcurrentBeacons;

        [SerializeField] private float maxTotalBeacons;
        
        [SerializeField]
        private TextMeshProUGUI beaconCountText;
        
        [SerializeField, ReadOnly] private int beaconCount;
        [SerializeField, ReadOnly] private int beaconTotal;


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {

                Instance = this;
            }
        }
        
        public void Spawn()
        {
            if (!(beaconCount < maxConcurrentBeacons) || !(beaconTotal < maxTotalBeacons))  return;

            var transformRef = transform;
            IncrementBeaconCount();
            Instantiate(beacon, transformRef.position, transformRef.rotation, transform);
        }

        public void OnBeaconDeath(Beacon deadBeacon)
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
            if (beaconCountText == null) return;
            beaconCountText.text = $"Beacons: {(beaconCount >= 0 ? beaconCount : 0)}";
        }
    }
}
