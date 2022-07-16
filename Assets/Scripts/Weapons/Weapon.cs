using UnityEngine;

namespace Weapons
{
    [CreateAssetMenu(menuName = "Weapon")]
    public class Weapon : ScriptableObject
    {
        public string weaponName;
        public GameObject modelPrefab;

        public string primaryAnimation;
        public string primaryComboAnimation;
        public string primaryCombo2Animation;

        public string specialAnimation;
    }
}
