using UnityEngine;

namespace PlayerBehaviors
{
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
