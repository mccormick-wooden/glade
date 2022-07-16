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

        private Player player;
        private PlayerWeaponManager playerWeaponManager;

        private Animator animator;
        private static readonly int IsAttacking = Animator.StringToHash("isAttacking");
        private static readonly int CanCombo = Animator.StringToHash("canCombo");

        #region Lock-On Properties

        public bool isLockingOn;
        public Transform currentLockedOnTarget;

        #endregion
        private void Start()
        {
            animator = GetComponent<Animator>();
            player = GetComponent<Player>();
            playerWeaponManager = GetComponent<PlayerWeaponManager>();

            isLockingOn = false;
        }

        private void Update()
        {
            ReadAnimatorParameters();
        }

        public void HandleLockOnInput()
        {
            Debug.Log("Handling lock on toggle");
            if (isLockingOn)
            {
                isLockingOn = false;
                currentLockedOnTarget = null;
                player.playerLockOnCamera.DisableLockOnCamera();
            }
            else
            {
                isLockingOn = true;
                AttemptLockOn();
            }
            
        }

        private void AttemptLockOn()
        {
            if (currentLockedOnTarget != null)
            {
                return;
            }

            var hits = Physics.SphereCastAll(transform.position, 10, transform.forward, 30f);
            foreach (var hit in hits)
            {
                var enemy = hit.collider.transform.root.GetComponent<BaseEnemy>();
                if (enemy != null)
                {
                    Debug.Log("Found an enemy");
                    currentLockedOnTarget = hit.collider.transform.root;
                }
            }

            if (currentLockedOnTarget != null)
            {
                // TODO: Player keeps a reference to this component
                player.playerLockOnCamera.EnableLockOnCamera(currentLockedOnTarget);
            }
        }

        private void ReadAnimatorParameters()
        {
            var wasAttacking = isAttacking;
            isAttacking = animator.GetBool(IsAttacking);

            // TODO: Remove this with animation events
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

        public void PerformHeavyAttack(Weapon weapon)
        {
            if (isAttacking)
            {
                return;
            }

            animator.SetBool(IsAttacking, true);

            var animationString = weapon.specialAnimation;
            SetRightHandInUse();
            animator.CrossFade(animationString, 0.2f);
            lastAttackPerformed = animationString;

            // TODO: Fix where this is
            playerWeaponManager.HandleSpecialEffect(weapon);
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

        // TODO: See update
        // To remove once we transition to dynamic colliders or some other way of knowing when a weapon is "in use"
        public void UnsetRightHandInUse()
        {
            if (playerWeaponManager.rightHandWeapon == null)
            {
                Utility.LogErrorIfNull(playerWeaponManager.rightHandWeapon, "rightHandWeapon",
                    "Could not find rightHandWeapon while Unsetting InUse");
                return;
            }

            playerWeaponManager.rightHandWeapon.InUse = false;
        }

        #endregion

        #region Lock-On

        #endregion
    }
}
