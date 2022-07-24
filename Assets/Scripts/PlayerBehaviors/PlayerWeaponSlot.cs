using UnityEngine;

namespace PlayerBehaviors
{
    // Place this behavior on a GameObject that should "hold" the weapon, like a model's hand
    public class PlayerWeaponSlot : MonoBehaviour
    {
        public GameObject weaponInSlot;

        public void EquipWeapon(PlayerWeapon weapon)
        {
            weaponInSlot = Instantiate(weapon.modelPrefab, transform);
            Utility.ResetLocalTransform(weaponInSlot.transform);
        }

        public void UnEquipWeapon()
        {
            if (weaponInSlot == null) return;
            weaponInSlot.SetActive(false);
        }
    }
}
