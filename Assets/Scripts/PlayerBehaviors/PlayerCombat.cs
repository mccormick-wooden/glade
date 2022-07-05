using UnityEngine;
using Weapons;

namespace PlayerBehaviors
{
    [RequireComponent(typeof(Player))]
    public class PlayerCombat : MonoBehaviour
    {
        public bool isAttacking;
        public string lastAttackPerformed;

        [SerializeField] private bool canCombo;

        private PlayerWeaponManager playerWeaponManager;

        private Animator animator;
        private static readonly int IsAttacking = Animator.StringToHash("isAttacking");
        private static readonly int CanCombo = Animator.StringToHash("canCombo");

        private void Start()
        {
            animator = GetComponent<Animator>();
            playerWeaponManager = GetComponent<PlayerWeaponManager>();
        }

        private void Update()
        {
            var wasAttacking = isAttacking;
            isAttacking = animator.GetBool(IsAttacking);
            if (wasAttacking && !isAttacking)
            {
                /*
                 * Workaround because of the InUse requirement for BaseWeapon and BaseDamageable
                 * Fine to leave for now, but we could replace it with a dynamic collider:
                 *  - use animation events to enable/disable the collider on the weapon only during appropriate frames
                 *  - makes it so it only collides when it would make sense to based on the animation
                 */
                UnsetRightHandInUse();
            }

            canCombo = animator.GetBool(CanCombo);
        }

        #region Sword Attacks

        public void PerformSlashAttack(Weapon weapon)
        {
            if (isAttacking && !canCombo)
            {
                return;
            }

            animator.applyRootMotion = false;
            animator.SetBool(IsAttacking, true);

            var animationString = "";
            if (canCombo)
            {
                if (lastAttackPerformed == weapon.primaryAnimation)
                {
                    animationString = weapon.primaryComboAnimation;
                }
                else if (lastAttackPerformed == weapon.primaryComboAnimation)
                {
                    animationString = weapon.primaryCombo2Animation;
                }
                else
                {
                    // Getting here means that we are currently animating the end of the combo chain
                    return;
                }
            }
            else
            {
                animationString = weapon.primaryAnimation;
            }

            // To remove once we transition to dynamic colliders or some other way of knowing when a weapon is "in use"
            SetRightHandInUse();
            animator.CrossFade(animationString, 0.2f);
            lastAttackPerformed = animationString;
        }

        #endregion

        #region Animator Handling

        // Set an animation event calling this function at the keyframe you would like to start accepting a combo attack
        public void OpenComboWindow()
        {
            animator.SetBool(CanCombo, true);
        }

        public void EnableAttacking()
        {
            animator.SetBool(IsAttacking, false);
        }

        private void SetRightHandInUse()
        {
            if (playerWeaponManager.rightHandWeapon == null)
            {
                Utility.LogErrorIfNull(playerWeaponManager.rightHandWeapon, "rightHandWeapon",
                    "Could not find rightHandWeapon while setting InUse");
                return;
            }

            playerWeaponManager.rightHandWeapon.InUse = true;
        }

        // To remove once we transition to dynamic colliders or some other way of knowing when a weapon is "in use"
        public void UnsetRightHandInUse()
        {
            if (playerWeaponManager.rightHandWeapon == null)
            {
                Utility.LogErrorIfNull(playerWeaponManager.rightHandWeapon, "rightHandWeapon",
                    "Could not find rightHandWeapon while UNsetting InUse");
                return;
            }

            playerWeaponManager.rightHandWeapon.InUse = false;
        }

        #endregion
    }
}
