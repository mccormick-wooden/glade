using UnityEngine;

namespace Beacons.dev
{
    public class TriggerBeaconSpawn : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.F))
            {
                Debug.Log("Spawning beacon...");
                BeaconSpawner.Instance.Spawn();
            }
        }
    }
}
