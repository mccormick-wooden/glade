using UnityEngine;

namespace Abstract
{
    public abstract class BaseSpawner: MonoBehaviour
    {
        public GameObject toSpawn;

        protected virtual void Start()
        {
            if (toSpawn != null) return;
            Debug.LogError("BaseSpawner expected toSpawn not to be null!");
        }

        public virtual void Spawn()
        {
            Instantiate(toSpawn, transform.position, transform.rotation);
        }
    }
}
