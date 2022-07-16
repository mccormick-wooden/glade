using System.Collections;
using Assets.Scripts.Abstract;
using UnityEngine;
using Weapons;

namespace PlayerBehaviors
{
    [RequireComponent(typeof(Player))]
    public class PlayerWeaponManager : MonoBehaviour
    {
        [SerializeField] private WeaponSlot leftHand;
        [SerializeField] private WeaponSlot rightHand;

        public BaseWeapon leftHandWeapon;
        public BaseWeapon rightHandWeapon;

        public GameObject specialEffectPrefab;

        private Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();
            Utility.LogErrorIfNull(animator, "animator",
                "PlayerWeaponManager expects its game object to have an Animator component");
        }

        private void Awake()
        {
            Utility.LogErrorIfNull(leftHand, "leftHand",
                "Could not find the leftHand WeaponSlot. Make sure to add a WeaponSlot component to this model's left hand object and assign it to the PlayerWeaponManager's leftHand variable in the editor.");
            Utility.LogErrorIfNull(rightHand, "rightHand",
                "Could not find the rightHand WeaponSlot. Make sure to add a WeaponSlot component to this model's right hand object and assign it to the PlayerWeaponManager's rightHand variable in the editor.");
        }

        public void EquipWeaponSlot(Weapon weapon, bool shouldLoadLeftHand)
        {
            if (shouldLoadLeftHand)
            {
                leftHand.EquipWeapon(weapon);
                leftHandWeapon = leftHand.weaponInSlot.GetComponentInChildren<BaseWeapon>();

            }
            else
            {
                rightHand.EquipWeapon(weapon);
                rightHandWeapon = rightHand.weaponInSlot.GetComponentInChildren<BaseWeapon>();
            }
        }

        public void HandleSpecialEffect(Weapon weapon)
        {
            if (weapon.specialAnimation == null)
            {
                return;
            }

            Debug.Log("instantiating");

            var specialEffectInstance = Instantiate(specialEffectPrefab, transform.position, transform.rotation);
            StartCoroutine(DestroySpecialEffect(specialEffectInstance));
        }

        private IEnumerator DestroySpecialEffect(GameObject instance)
        {
            yield return new WaitForSeconds(3);
            Destroy(instance);
        }
    }
}
