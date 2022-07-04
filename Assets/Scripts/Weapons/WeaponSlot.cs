using UnityEngine;

namespace Weapons
{
    public class WeaponSlot : MonoBehaviour
    {
        // Where are we actually holding the weapon?
        public Transform weaponSlotTransform;
        public GameObject weaponInSlot;

        public void EquipWeapon(Weapon weapon)
        {
            weaponInSlot = Instantiate(weapon.modelPrefab, transform);

            if (weaponInSlot == null)
            {
                Utility.LogErrorIfNull(weaponInSlot, "weaponInSlot",
                    "Instantiated a weapon but weaponInSlot still null!");
                return;
            } 

            Utility.ResetLocalTransform(weaponInSlot.transform);
        }

        public void UnEquipWeapon()
        {
            if (weaponInSlot == null) return;
            weaponInSlot.SetActive(false);
        }
    }
}
