using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Abstract;
using Assets.Scripts.Interfaces;
using UnityEngine;

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
        private LayerMask playerLayerMask;

        private Animator animator;
        private static readonly int IsAttacking = Animator.StringToHash("isAttacking");
        private static readonly int CanCombo = Animator.StringToHash("canCombo");

        public bool isLockingOn;
        public Transform currentLockedOnTarget;

        [SerializeField] private float maxLockOnDistance;

        public float windAttackMana = 10f;
        public float manaRechargeDelay = 2;
        public float manaRechargePerSecond = 5;
        public float lastCastTime = 0f;
        public HealthBarController manaBar;
        public AudioSource castFailSound;
        
        private void Awake()
        {
            playerLayerMask = LayerMask.GetMask("Player");
        }

        private void Start()
        {
            if (manaBar == null)
                Debug.LogError("Couldn't find mana bar");
            animator = GetComponent<Animator>();
            player = GetComponent<Player>();
            playerWeaponManager = GetComponent<PlayerWeaponManager>();

            isLockingOn = false;
            currentLockedOnTarget = null;

            if (maxLockOnDistance == 0)
            {
                maxLockOnDistance = 30f;
            }
        }

        private void UpdateMana()
        {
            if (Time.time - lastCastTime >= manaRechargeDelay)
            {
                manaBar.CurrentHp += manaRechargePerSecond * Time.deltaTime;
            }
        }

        private void Update()
        {
            UpdateMana();
            ReadAnimatorParameters();
            DetectLockOnOutOfRange();
            // DetectLockOnOutOfSight(); Reintroduce if we want to break lock-on when losing line of sight
        }

        #region Lock On

        public void ToggleLockOn()
        {
            if (isLockingOn)
            {
                isLockingOn = false;
                StopListeningToTargetDeath();

                currentLockedOnTarget = null;
                player.playerLockOnCamera.DisableLockOnCamera();
            }
            else
            {
                AttemptLockOn();
            }
            
        }

        private void AttemptLockOn()
        {
            // Get the closest
            var target = GetClosestLockOnTarget();

            if (target == null)
            {
                isLockingOn = false;
            }
            else
            {
                currentLockedOnTarget = target;
                isLockingOn = true;
                var damageable = currentLockedOnTarget.GetComponent<BaseDamageable>();
                if (damageable != null)
                {
                    damageable.Died += HandleDeadLockOnTarget;
                }
                player.playerLockOnCamera.EnableLockOnCamera(currentLockedOnTarget);
            }
        }

        private Transform GetClosestLockOnTarget()
        {
            var minDistance = Mathf.Infinity;
            Transform closestTarget = null;

            var hits = Physics.SphereCastAll(transform.position, 10, transform.forward, 30f, ~playerLayerMask);
            foreach (var hit in hits)
            {
                var enemy = hit.collider.transform.GetComponent<BaseDamageable>();
                if (enemy != null)
                {
                    if (enemy.IsDead)
                    {
                        continue;
                    }
                    
                    // Handle finding a potential target
                    var distance = Vector3.Distance(transform.position, enemy.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestTarget = enemy.transform;
                    }
                }
            }

            return closestTarget;
        }

        private Transform GetRelativeLockOnTarget(bool isLeft)
        {
            var lockOnCandidates = new List<Transform>();
            var hits = Physics.SphereCastAll(transform.position, 10, transform.forward, 30f);
            foreach (var hit in hits)
            {
                // Maybe switch to damageable to include beacons, crystals?
                var enemy = hit.collider.transform.GetComponent<BaseEnemy>();
                if (enemy != null)
                {
                    // Handle finding a potential target
                    lockOnCandidates.Add(hit.collider.transform);
                }
            }

            // If there's only one option we'll take it
            if (lockOnCandidates.Count == 1)
            {
                return lockOnCandidates[0].transform;
            }

            // If we fail to find any candidates, stick with the one we have
            if (lockOnCandidates.Count == 0)
            {
                return currentLockedOnTarget;
            }

            lockOnCandidates.Sort(SortTransformsByRelativeX);

            var indexOfCurrentLockOnTarget = lockOnCandidates.FindIndex(t =>
                t.gameObject.GetInstanceID() == currentLockedOnTarget.gameObject.GetInstanceID());

            var adjustment = isLeft ? -1 : 1;
            var adjustedIndex = indexOfCurrentLockOnTarget + adjustment;

            if (isLeft && adjustedIndex < 0)
            {
                adjustedIndex = lockOnCandidates.Count - 1;
            }

            if (!isLeft && adjustedIndex == lockOnCandidates.Count())
            {
                adjustedIndex = 0;
            }

            return lockOnCandidates[adjustedIndex];
        }

        private void HandleDeadLockOnTarget(IDamageable damageable, string objectName, int id)
        {
            if (isLockingOn)
            {
                // If we were locking on and the target died let's re-lock to a new target
                var newTarget = GetClosestLockOnTarget();
                if (newTarget == null)
                {
                    // Turn off lock-on if we couldn't find a new target
                    ToggleLockOn();
                }
                else
                {
                    SwitchTargets(newTarget);
                }
            }
        }

        private int SortTransformsByRelativeX(Transform a, Transform b)
        {
            var relativePositionToPlayerA = transform.InverseTransformPoint(a.position);
            var relativePositionToPlayerB = transform.InverseTransformPoint(b.position);

            if (relativePositionToPlayerB.x == relativePositionToPlayerA.x)
            {
                return 0;
            }

            if (relativePositionToPlayerA.x > relativePositionToPlayerB.x)
            {
                return 1;
            }

            return -1;
        }
        
        public void HandleLockOnCycle(bool isLeft)
        {
            if (!isLockingOn)
            {
                return;
            }

            var newTarget = GetRelativeLockOnTarget(isLeft);

            if (newTarget != null)
            {
                StopListeningToTargetDeath();
                SwitchTargets(newTarget);
            }
        }

        private void SwitchTargets(Transform newTarget)
        {
            currentLockedOnTarget = newTarget;
            var damageable = currentLockedOnTarget.GetComponent<BaseDamageable>();
            if (damageable != null)
            {
                damageable.Died += HandleDeadLockOnTarget;
            }

            player.transform.LookAt(currentLockedOnTarget);
            player.playerLockOnCamera.UpdateLockOnCameraLookAt(currentLockedOnTarget);
        }

        // We should stop locking on if the lock on target gets to far away
        private void DetectLockOnOutOfRange()
        {
            if (!isLockingOn)
            {
                return;
            }

            var distance = Vector3.Distance(transform.position, currentLockedOnTarget.position);
            if (distance > maxLockOnDistance)
            {
                Debug.LogWarning("LockOn Exit Due to Out of Range");
                ToggleLockOn();
            }
        }

        private void StopListeningToTargetDeath()
        {
            if (currentLockedOnTarget != null)
            {
                var damageable = currentLockedOnTarget.GetComponent<BaseDamageable>();
                if (damageable != null)
                {
                    damageable.Died -= HandleDeadLockOnTarget;
                }
            }
        }

        #endregion

        #region Sword Attacks

        public void PerformSlashAttack(PlayerWeapon weapon)
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

        public void PerformSpecialAttack(PlayerWeapon weapon)
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
        }

        private IEnumerator DestroySpecialEffect(GameObject instance)
        {
            yield return new WaitForSeconds(3);
            Destroy(instance);
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

        public void CastWindAttack()
        {
            var weapon = player.primaryWeapon;

            if (weapon.specialAttackPrefab != null)
            {
                if (manaBar?.CurrentHp >= windAttackMana)
                {
                    manaBar.CurrentHp -= windAttackMana;
                    lastCastTime = Time.time;
                    var specialEffectInstance =
                        Instantiate(weapon.specialAttackPrefab, transform.position, transform.rotation);
                    StartCoroutine(DestroySpecialEffect(specialEffectInstance));
                }
                else
                {
                    castFailSound.Play();
                }
            }
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
