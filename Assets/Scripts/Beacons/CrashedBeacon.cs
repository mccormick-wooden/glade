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

        private IDamageable _damageable;

        private void Awake()
        {
            _damageable = GetComponent<IDamageable>();
            Utility.LogErrorIfNull(_damageable, "_damageable",
                "CrashedBeacon must have a component that implements IDamageable");
            if (_damageable != null)
            {
                _damageable.Died += OnDied;
            }

            Utility.LogErrorIfNull(powerUpPrefab, "powerUpPrefab",
                "The CrashedBeacon will not turn into a powerUpPickup without a prefab to Instantiate");
        }

        private void OnDied(IDamageable damageable, string name, int id)
        {
            damageable.Died -= OnDied;
            DropPowerUp();
        }

        private void DropPowerUp()
        {
            if (powerUpPrefab == null) return;
            Instantiate(powerUpPrefab,
                transform.position,
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
        }

        private void HoverAndRotate()
        {
            transform.Rotate(new Vector3(0f, Time.deltaTime * rotationSpeed, 0f));

            var newPosition = position;
            newPosition.y += Mathf.Sin(Time.fixedTime * Mathf.PI * hoverFrequency) * hoverAmplitude;

            transform.position = newPosition;
        }
    }
}
