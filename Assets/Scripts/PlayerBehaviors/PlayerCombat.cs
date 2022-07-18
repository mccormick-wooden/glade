using System.Collections.Generic;
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

        public bool isLockingOn;
        public Transform currentLockedOnTarget;

        private int currentLockedOnTargetIndex = -1;
        private List<Transform> lockOnCandidates;
        
        private void Start()
        {
            animator = GetComponent<Animator>();
            player = GetComponent<Player>();
            playerWeaponManager = GetComponent<PlayerWeaponManager>();

            isLockingOn = false;
            currentLockedOnTarget = null;
            lockOnCandidates = new List<Transform>();
        }

        private void Update()
        {
            ReadAnimatorParameters();
        }

        #region Lock On

        public void HandleLockOnToggle()
        {
            Debug.Log("Handling lock on toggle");
            if (isLockingOn)
            {
                isLockingOn = false;
                currentLockedOnTarget = null;
                lockOnCandidates = new List<Transform>();
                player.playerLockOnCamera.DisableLockOnCamera();
            }
            else
            {
                AttemptLockOn();
            }
            
        }

        private void AttemptLockOn()
        {
            if (currentLockedOnTarget != null)
            {
                return;
            }

            var seen = new HashSet<GameObject>();
            var hits = Physics.SphereCastAll(transform.position, 10, transform.forward, 30f);
            foreach (var hit in hits)
            {
                // Maybe switch to damageable?
                var enemy = hit.collider.transform.GetComponent<BaseEnemy>();
                if (enemy != null)
                {
                    var hitTransform = hit.collider.transform;
                    currentLockedOnTarget = hitTransform;

                    Debug.Log(hitTransform.gameObject.name);

                    if (seen.Add(hitTransform.gameObject))
                    {
                        lockOnCandidates.Add(hit.collider.transform);
                    }
                }
            }

            if (currentLockedOnTarget == null)
            {
                isLockingOn = false;
            }
            else
            {
                Debug.Log(lockOnCandidates.Count);
                
                isLockingOn = true;
                // TODO: Player keeps a reference to this component
                player.playerLockOnCamera.EnableLockOnCamera(currentLockedOnTarget);
            }
        }

        public void HandleLockOnCycle(bool isLeft)
        {
            if (lockOnCandidates.Count <= 1 || !isLockingOn)
            {
                return;
            }

            var tempCandidateIndex = currentLockedOnTargetIndex;
            tempCandidateIndex += isLeft ? -1 : 1;

            currentLockedOnTargetIndex = tempCandidateIndex < 0
                ? lockOnCandidates.Count - 1
                : tempCandidateIndex >= lockOnCandidates.Count
                    ? 0
                    : tempCandidateIndex;

            currentLockedOnTarget = lockOnCandidates[currentLockedOnTargetIndex];

            // This rotation should be smoothed at a minimum, animated would be even better
            player.transform.LookAt(currentLockedOnTarget);
            player.playerLockOnCamera.UpdateLockOnCameraLookAt(currentLockedOnTarget);
        }

        #endregion

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

        #region Combat Feedback

        // Sounds are triggered by animation events
        public void EmitFirstSwordSlash()
        {
            EventManager.TriggerEvent<SwordSwingEvent, Vector3, int>(transform.position, 0);
        }

        public void EmitSecondSwordSlash()
        {
            EventManager.TriggerEvent<SwordSwingEvent, Vector3, int>(transform.position, 1);
        }

        public void EmitThirdSwordSlash()
        {
            EventManager.TriggerEvent<SwordSwingEvent, Vector3, int>(transform.position, 2);
        }

        #endregion
    }
}
