using System;
using Assets.Scripts.Abstract;
using DigitalRuby.PyroParticles;
using UnityEngine;

namespace Beacons
{
    public class Beacon : BaseDamageable
    {
        public GameObject fallingBeaconObject;
        public GameObject crashedBeaconObject;

        private GameObject fallingBeacon;
        private GameObject crashedBeacon;

        private Rigidbody fallingBeaconRigidBody;
        private FireProjectileScript beaconFall;

        private bool _isCrashed;
        private bool _isBeaconSpawned;
        
        private void Awake()
        {
            if (fallingBeaconObject == null) Debug.LogError("Beacon does not have an object to represent fallingBeacon!");
        }

        protected override void Start()
        {
            base.Start();
            _isCrashed = false;
            _isBeaconSpawned = false;

            if (fallingBeaconObject == null)
            {
                Debug.LogError("Could not find fallingBeaconObject");
            }
            
            fallingBeacon = Instantiate(fallingBeaconObject, transform.position, transform.rotation, transform);
            beaconFall = GetComponentInChildren<FireProjectileScript>();

            if (beaconFall == null)
            {
                Debug.LogError("Could not find component BeaconFall");
                _isCrashed = true;
            }
            
            fallingBeaconRigidBody = fallingBeacon.GetComponentInChildren<Rigidbody>();
            if (fallingBeaconRigidBody == null)
            {
                Debug.LogError("The falling beacon does not have a rigid body!");
            }

            if (fallingBeacon == null)
            {
                Debug.LogError("Could not find fallingBeacon");
            }
        }

        private void LateUpdate()
        {
            _isCrashed = beaconFall.isCrashed;

            if (!_isCrashed || _isBeaconSpawned) return;
            
            crashedBeacon = Instantiate(crashedBeaconObject, fallingBeaconRigidBody.transform.position, Quaternion.LookRotation(transform.position, Vector3.up), transform);
            _isCrashed = true;
            _isBeaconSpawned = true;
        }

        protected override void Die()
        {
            base.Die();
            BeaconSpawner.Instance.OnBeaconDeath(this);
        }
    }
}
