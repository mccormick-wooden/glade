using Assets.Scripts.Abstract;
using UnityEngine;

namespace PlayerBehaviors
{
    [CreateAssetMenu(menuName = "PlayerWeapon")]
    public class PlayerWeapon : ScriptableObject
    {
        public GameObject modelPrefab;

        public string primaryAnimation;
        public string primaryComboAnimation;
        public string primaryCombo2Animation;

        public string specialAnimation;
        public GameObject specialAttackPrefab;
    }
}
