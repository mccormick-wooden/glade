using Assets.Scripts.Interfaces;
using UnityEngine;
using System.Linq;
using System;

namespace Assets.Scripts.Abstract
{
    public abstract class BaseDamageable : MonoBehaviour, IDamageable
    {
        [SerializeField] protected HealthBarController healthBarController;

        [SerializeField] private float currentHp;

        [SerializeField] private float maxHp = 100;

        [SerializeField] public bool IsHealable;

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

        public int AttachedInstanceId { get; protected set; }

        public virtual bool IsDead { get; protected set; } = false;

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
                Debug.Log(
                    $"Healing {gameObject.name}: currentHp = {CurrentHp}, healAmount: {healAmount}, newHp = {newHp}");
                CurrentHp = newHp;
            }
        }

        public Action<IDamageable, string, int> Died { get; set; }

        protected virtual void Start()
        {
            CurrentHp = MaxHp;

            if (healthBarController == null)
                healthBarController = GetComponentInChildren<HealthBarController>();
            if (healthBarController != null)
                healthBarController.InitHealthBar(CurrentHp);

            Debug.Log(
                healthBarController == null
                    ? $"{gameObject.name} HP set to {CurrentHp}/{MaxHp}"
                    : $"{gameObject.name} HP set to {CurrentHp}/{MaxHp} with health bar at {healthBarController.CurrentHp}/{healthBarController.MaxHp}"
            );

            AttachedInstanceId = gameObject.GetInstanceID();
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (ShouldHandleCollisionAsAttack(other))
                HandleAttack(other.GetComponent<BaseWeapon>());
        }

        protected void HandleAttack(BaseWeapon attackingWeapon)
        {
            ApplyDamage(attackingWeapon);

            if (healthBarController != null)
                healthBarController.CurrentHp = CurrentHp;

            if (!HasHp)
                Die();
        }

        protected virtual bool ShouldHandleCollisionAsAttack(Collider other)
        {
            var attackingWeapon = other.GetComponent<BaseWeapon>();
            if (attackingWeapon == null)
                return false;

            bool isWeaponTarget = attackingWeapon.TargetTags.Contains(transform.tag);
            return attackingWeapon.InUse && HasHp && isWeaponTarget;
        }

        protected virtual void ApplyDamage(BaseWeapon attackingWeapon, float modifier = 1f)
        {
            var netAttackDamage = attackingWeapon.AttackDamage * modifier;
            var newHp = Mathf.Max(CurrentHp - netAttackDamage, 0f);
            Debug.Log(
                $"Applying damage to {gameObject.name}: currentHp = {CurrentHp}, damage = {netAttackDamage}, newHp = {newHp}"
            );
            CurrentHp = newHp;
        }

        public void UpdateMaxHealth(float scalar)
        {
            var oldMaxHp = MaxHp;
            var newMaxHp = Mathf.Floor(oldMaxHp * scalar);
            MaxHp = newMaxHp < 1 ? 1 : newMaxHp;

            healthBarController.MaxHp = MaxHp;
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
            Debug.Log("Changed Current HP from " + oldCurrentHp + " to " + CurrentHp);
        }
        
        /// <summary>
        /// If overridden, base implementation MUST be called
        /// </summary>
        protected virtual void Die()
        {
            Died?.Invoke(this, name, AttachedInstanceId);
            IsDead = true;
        }
    }
}
