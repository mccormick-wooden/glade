using UnityEngine;

namespace Beacons
{
    public class BeaconFall : MonoBehaviour
    {
        public bool isCrashed;
        private Collider fireboltCollider;

        private void Awake()
        {
            isCrashed = false;
            fireboltCollider = GetComponentInChildren<Collider>();
        }

        private void OnCrash()
        {
            Debug.Log("Beacon crashed into the Glade!");
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Terrain")) return;

            isCrashed = true;
            OnCrash();
        }

    }
}
