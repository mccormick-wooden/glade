using Assets.Scripts.Interfaces;
using UnityEngine;

/* 
* Please note that the `FireProjectileScript.cs` depends on the CrashedBeacon class to find crashed beacon game objects**
*/
namespace Beacons
{
    public class CrashedBeacon : MonoBehaviour
    {
        public GameObject powerUpPrefab;

        private Vector3 position;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float hoverFrequency;
        [SerializeField] private float hoverAmplitude;

        private BeaconOvershield overShield;
        
        // The radius of the sphere used to look for enemies and crystals around the beacons
        [SerializeField] private float detectionRadius;
        [SerializeField] private float minTimeToKeepShieldsUp;

        // The minimum ratio of enemies:max enemies around the beacon to keep the shield up
        [SerializeField] private float minEnemyRatio;
        private uint maxPossibleEnemiesPerBeacon;
        
        private IDamageable _damageable;

        // Keep a reference to this so we can keep our shields up based on the ratio of living enemies as a function
        // of the maximum that can spawn around the beacon
        private EnemySpawner playerEnemySpawner;

        private float minY = float.MaxValue;
        private float maxY = float.MinValue;

        private void Awake()
        {
            _damageable = GetComponent<IDamageable>();
            Utility.LogErrorIfNull(_damageable, "_damageable",
                "CrashedBeacon must have a component that implements IDamageable");
            if (_damageable != null)
            {
                _damageable.Died += OnDied;
            }

            _damageable.IsImmune = true;

            Utility.LogErrorIfNull(powerUpPrefab, "powerUpPrefab",
                "The CrashedBeacon will not turn into a powerUpPickup without a prefab to Instantiate");

            overShield = GetComponentInChildren<BeaconOvershield>();
            playerEnemySpawner = FindObjectOfType<Player>()?.GetComponent<EnemySpawner>();

            maxPossibleEnemiesPerBeacon = playerEnemySpawner != null
                ? playerEnemySpawner.GetMaximumNumberOfEnemiesPerBeacon()
                : (uint)10; // Arbitrary default, shouldn't happen
        }

        private void OnDied(IDamageable damageable, string name, int id)
        {
            damageable.Died -= OnDied;
            DropPowerUp();
        }

        private void DropPowerUp()
        {
            if (powerUpPrefab == null) return;

            var pos = transform.position;

            pos.y = (minY + maxY) / 2;

            Instantiate(powerUpPrefab,
                pos,
                Quaternion.identity, transform.parent);
        }

        private void Start()
        {
            position = transform.position;
            position.y += hoverAmplitude;
        }

        private void Update()
        {
            HoverAndRotate();
            CheckForNearbyEnemies();
        }

        /* If the beacon can't find any crystal within the detection range it lowers its shields  */
        private void CheckForNearbyEnemies()
        {
            if (minTimeToKeepShieldsUp > 0)
            {
                minTimeToKeepShieldsUp -= Time.deltaTime;
            } else {
                var enemyCount = 0;
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
                foreach (var hitCollider in hitColliders)
                {
                    var enemy = hitCollider.gameObject.GetComponent<BaseEnemy>();
                    var crystal = hitCollider.gameObject.GetComponent<CrystalController>();

                    // CrystalControllers stay active after death so we need to drill down a little further to ensure
                    // the underlying damageable crystal is dead if we are going to lower shields
                    if (crystal != null) {
                        var damageable = crystal.gameObject.GetComponentInChildren<IDamageable>();
                        if (!damageable.IsDead) {
                            return;
                        }
                    }

                    if (enemy != null) {
                        enemyCount++;
                    }
                }

                var minEnemyCount = Mathf.FloorToInt(minEnemyRatio * maxPossibleEnemiesPerBeacon);
                if (enemyCount < minEnemyCount || enemyCount == 0)
                {
                    DisableOverShield();
                }
            }
        }

        private void DisableOverShield()
        {
            _damageable.IsImmune = false;
            overShield.gameObject.SetActive(false);
        }

        private void HoverAndRotate()
        {
            transform.Rotate(new Vector3(0f, Time.deltaTime * rotationSpeed, 0f));

            var newPosition = position;
            newPosition.y += Mathf.Sin(Time.fixedTime * Mathf.PI * hoverFrequency) * hoverAmplitude;

            minY = newPosition.y < minY ? newPosition.y : minY;
            maxY = newPosition.y > maxY ? newPosition.y : maxY;

            transform.position = newPosition;
        }
    }
}
