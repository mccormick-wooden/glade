using System.Collections;
using Assets.Scripts.Abstract;
using UnityEngine;

namespace PlayerBehaviors
{
    [RequireComponent(typeof(Player))]
    public class PlayerWeaponManager : MonoBehaviour
    {
        [SerializeField] private PlayerWeaponSlot leftHand;
        [SerializeField] private PlayerWeaponSlot rightHand;

        public BaseWeapon leftHandWeapon;
        public BaseWeapon rightHandWeapon;

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

        public void EquipWeaponSlot(PlayerWeapon weapon, bool shouldLoadLeftHand)
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
    }
}
