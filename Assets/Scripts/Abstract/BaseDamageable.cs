﻿using Assets.Scripts.Interfaces;
using UnityEngine;
using System.Linq;
using System;

namespace Assets.Scripts.Abstract
{
    public abstract class BaseDamageable : MonoBehaviour, IDamageable
    {
        [SerializeField] protected HealthBarController healthBarController;

        [Tooltip("Optional GameObject to use as name/id for death event invokation.")]
        [SerializeField] private GameObject attachedObject = null;

        [SerializeField] private float currentHp;

        [SerializeField] private float maxHp = 100;

        [SerializeField] protected bool debugOutput = true;

        private MinimapIcon minimapIcon;

        public bool IsHealable { get; set; }

        public float CurrentHp
        {
            get => currentHp;
            protected set => currentHp = value;
        }

        public float MaxHp
        {
            get => maxHp;
            protected set => maxHp = value;
        }

        public bool HasHp => CurrentHp > 0;

        protected virtual bool UseHealthBarText { get; set; } = false;

        public int AttachedInstanceId { get; protected set; }

        public string AttachedName { get; protected set; }

        public virtual bool IsDead { get; protected set; } = false;

        public Action<IDamageable, string, int> Died { get; set; }

        public bool IsImmune { get; set; } = false;

        protected virtual void Start()
        {
            CurrentHp = MaxHp;

            InitHealthBarIfExists();

            if (debugOutput)
            {
                Debug.Log(
                    healthBarController == null
                        ? $"{gameObject.name} HP set to {CurrentHp}/{MaxHp}"
                        : $"{gameObject.name} HP set to {CurrentHp}/{MaxHp} with health bar at {healthBarController.CurrentHp}/{healthBarController.MaxHp}");
            }

            AttachedInstanceId = gameObject.GetInstanceID();
            AttachedName = gameObject.name;
            if (attachedObject != null)
            {
                AttachedInstanceId = attachedObject.GetInstanceID();
                AttachedName = attachedObject.name;
            }
            GetMinimapIcon();
        }

        public virtual void Heal(float healAmount)
        {
            if (healAmount < 0)
            {
                Debug.LogError($"{name}: Tried to heal with negative value {healAmount}");
                return;
            }

            if (!IsDead && IsHealable)
            {
                var newHp = Mathf.Min(CurrentHp + healAmount, MaxHp);

                if (debugOutput)
                {
                    Debug.Log(
                       $"Healing {gameObject.name}: currentHp = {CurrentHp}, healAmount: {healAmount}, newHp = {newHp}");
                }

                CurrentHp = newHp;

                if (healthBarController != null)
                    healthBarController.CurrentHp = CurrentHp;
            }
        }

        public virtual void HandleAttack(IWeapon attackingWeapon)
        {
            if (!IsImmune) // check again here because CrystalWeapon skips the trigger
            {
                ApplyDamage(attackingWeapon);

                if (healthBarController != null)
                    healthBarController.CurrentHp = CurrentHp;

                if (!HasHp)
                    Die();
            }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            var attackingWeapon = other.GetComponent<BaseWeapon>();

            if (!attackingWeapon || attackingWeapon.isDPSType)
            {
                return;
            }

            if (ShouldHandleCollisionAsAttack(attackingWeapon) && !IsImmune)
            {
                HandleAttack(attackingWeapon);
            }
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            var attackingWeapon = other.GetComponent<BaseWeapon>();

            if (!attackingWeapon)
            {
                return;
            }

            // short circuit on AOEType to get out quick if not AOE
            if (attackingWeapon && attackingWeapon.isDPSType && ShouldHandleCollisionAsAttack(attackingWeapon))
            {
                HandleAttack(attackingWeapon);
            }
        }

        protected virtual bool ShouldHandleCollisionAsAttack(BaseWeapon attackingWeapon)
        {
            bool isWeaponTarget = attackingWeapon.TargetTags.Contains(transform.tag);
            return attackingWeapon.InUse && HasHp && isWeaponTarget;
        }

        protected virtual void ApplyDamage(IWeapon attackingWeapon, float modifier = 1f)
        {
            var netAttackDamage = attackingWeapon.AttackDamage * modifier;
            if (attackingWeapon.isDPSType)
            {
                netAttackDamage *= Time.deltaTime;
            }

            var newHp = Mathf.Max(CurrentHp - netAttackDamage, 0f);

            if (debugOutput)
            {
                Debug.Log(
                    $"Applying damage to {gameObject.name}: currentHp = {CurrentHp}, damage = {netAttackDamage}, newHp = {newHp}"
                );
            }

            CurrentHp = newHp;
        }

        public void UpdateMaxHealth(float scalar)
        {
            var oldMaxHp = MaxHp;
            var newMaxHp = Mathf.Floor(oldMaxHp * scalar);
            MaxHp = newMaxHp < 1 ? 1 : newMaxHp;

            healthBarController.MaxHp = MaxHp;

            if (debugOutput)
                Debug.Log("Changed Max HP from " + oldMaxHp + " to " + MaxHp);

            var oldCurrentHp = CurrentHp;
            if (scalar >= 1f)
            {
                // This is opinionated so this can change:
                // but the health points "gained" from the new MaxHp are applied as healing
                var diff = MaxHp - oldMaxHp;
                CurrentHp += diff;
            }
            else
            {
                // Also opinionated:
                // Losing max health only lowers our existing health if the max is lower than current hp
                if (CurrentHp > MaxHp)
                {
                    CurrentHp = MaxHp;
                }
            }

            healthBarController.CurrentHp = CurrentHp;

            if (debugOutput)
                Debug.Log("Changed Current HP from " + oldCurrentHp + " to " + CurrentHp);
        }

        /// <summary>
        /// If overridden, base implementation MUST be called
        /// </summary>
        protected virtual void Die()
        {
            minimapIcon?.Disable();
            IsDead = true;
            Died?.Invoke(this, AttachedName, AttachedInstanceId);
        }

        private void InitHealthBarIfExists()
        {
            if (healthBarController == null)
                healthBarController = GetComponentInChildren<HealthBarController>();

            if (healthBarController == null)
                healthBarController = gameObject.transform.parent?.gameObject?.GetComponentInChildren<HealthBarController>();

            if (healthBarController != null)
                healthBarController.InitHealthBar(CurrentHp, UseHealthBarText);
        }

        private void GetMinimapIcon()
        {
            minimapIcon = GetComponentInChildren<MinimapIcon>();

            if (minimapIcon == null)
                gameObject.transform.parent?.GetComponentInChildren<MinimapIcon>();
        }

        public virtual void InstantKill()
        {
            currentHp = 0;
            Die();
        }
    }
}
