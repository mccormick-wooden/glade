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

        public Action<BeaconManager, GameObject> BeaconReadyForDamage { get; set; } 

        private void Start()
        {
            Utility.LogErrorIfNull(fallingBeaconPrefab, nameof(fallingBeaconPrefab), "Need an object to represent falling beacon!");

            fallingBeaconInstance = Instantiate(fallingBeaconPrefab, transform.position, transform.rotation, transform);
            Utility.LogErrorIfNull(fallingBeaconInstance, nameof(fallingBeaconInstance));

            fallingBeaconRigidBody = fallingBeaconInstance.GetComponentInChildren<Rigidbody>();
            Utility.LogErrorIfNull(fallingBeaconRigidBody, nameof(fallingBeaconRigidBody), "Needs a rigidbody so that collision with terrain works!");

            beaconFall = GetComponentInChildren<FireProjectileScript>();
            Utility.LogErrorIfNull(beaconFall, nameof(beaconFall));

            beaconFall.CollisionDelegate += OnBeaconLanded;
        }

        /// <summary>
        /// Callback for the FireProjectileScript's collision delegate.
        /// This effectivly tells us when the projectile (beacon) experiences a collision event,
        /// which for beacons mean they have landed.
        /// </summary>
        /// <param name="script"></param>
        /// <param name="pos"></param>
        private void OnBeaconLanded(FireProjectileScript script, Vector3 pos)
        {
            beaconFall.CollisionDelegate -= OnBeaconLanded; 
            crashedBeaconInstance = Instantiate(crashedBeaconPrefab, fallingBeaconRigidBody.transform.position, Quaternion.LookRotation(transform.position, Vector3.up), transform);
            GameObject.Find("PlayerModel").GetComponent<EnemySpawner>().AddBeacon(crashedBeaconInstance);
            BeaconReadyForDamage?.Invoke(this, crashedBeaconInstance);
        }
    }
}
