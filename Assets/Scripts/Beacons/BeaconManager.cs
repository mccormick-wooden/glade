using System;
using Assets.Scripts.Abstract;
using DigitalRuby.PyroParticles;
using UnityEngine;

namespace Beacons
{
    public class BeaconManager : MonoBehaviour
    {
        public GameObject fallingBeaconPrefab;
        public GameObject crashedBeaconPrefab;

        /* These will be the instances of the respective prefabs */
        private GameObject fallingBeaconInstance;
        private GameObject crashedBeaconInstance;

        private Rigidbody fallingBeaconRigidBody;
        private FireProjectileScript beaconFall;

        private bool _isCrashed;
        private bool _isBeaconSpawned;

        private void Awake()
        {
            if (fallingBeaconPrefab == null) Debug.LogError("Beacon does not have an object to represent fallingBeacon!");
        }

        private void Start()
        {
            _isCrashed = false;
            _isBeaconSpawned = false;

            if (fallingBeaconPrefab == null)
            {
                Debug.LogError("Could not find fallingBeaconObject");
            }

            fallingBeaconInstance = Instantiate(fallingBeaconPrefab, transform.position, transform.rotation, transform);
            if (fallingBeaconInstance == null)
            {
                Debug.LogError("Could not find fallingBeacon");
            }

            beaconFall = GetComponentInChildren<FireProjectileScript>();
            if (beaconFall == null)
            {
                Debug.LogError("Could not find component BeaconFall");
                _isCrashed = true;
            }

            fallingBeaconRigidBody = fallingBeaconInstance.GetComponentInChildren<Rigidbody>();
            if (fallingBeaconRigidBody == null)
            {
                /* Need a rigid body so that collision with terrain works */
                Debug.LogError("The falling beacon does not have a rigid body!");
            }
        }

        private void LateUpdate()
        {
            _isCrashed = beaconFall.isCrashed;

            if (!_isCrashed || _isBeaconSpawned) return;

            crashedBeaconInstance = Instantiate(crashedBeaconPrefab, fallingBeaconRigidBody.transform.position, Quaternion.LookRotation(transform.position, Vector3.up), transform);
            GameObject.Find("Player").GetComponent<EnemySpawner>().AddBeacon(crashedBeaconInstance);
            _isCrashed = true;
            _isBeaconSpawned = true;
        }
    }
}
