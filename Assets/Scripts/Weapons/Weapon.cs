using UnityEngine;

namespace Weapons
{
    [CreateAssetMenu(menuName = "Weapon")]
    public class Weapon : ScriptableObject
    {
        public string weaponName;
        public GameObject modelPrefab;
    }
}
