using Assets.Scripts.Interfaces;
using UnityEngine;
using System.Linq;
using System;

namespace Assets.Scripts.Abstract
{
    public abstract class BaseDamageable : MonoBehaviour, IDamageable
    {
        [SerializeField]
        protected HealthBarController healthBarController;

        [SerializeField]
        private float currentHp;

        [SerializeField]
        private float maxHp = 100;

        [SerializeField]
        public bool IsHealable;

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
                Debug.Log($"Healing {gameObject.name}: currentHp = {CurrentHp}, healAmount: {healAmount}, newHp = {newHp}");
                CurrentHp = newHp;
            }
        }

        public Action<IDamageable, string, int> Died { get; set; }

        protected virtual void Start()
        {
            CurrentHp = MaxHp;

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
            var attackingWeapon = other.GetComponent<BaseWeapon>();

            if (!attackingWeapon || attackingWeapon.isDPSType)
                return;

            if (ShouldHandleCollisionAsAttack(attackingWeapon))
                HandleAttack(attackingWeapon);
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            var attackingWeapon = other.GetComponent<BaseWeapon>();

            if (!attackingWeapon)
                return;

            // short circuit on AOEType to get out quick if not AOE
            if (attackingWeapon && attackingWeapon.isDPSType && ShouldHandleCollisionAsAttack(attackingWeapon))
                HandleAttack(attackingWeapon);
        }

        protected void HandleAttack(BaseWeapon attackingWeapon)
        {
            ApplyDamage(attackingWeapon);

            if (healthBarController != null)
                healthBarController.CurrentHp = CurrentHp;

            if (!HasHp)
                Die();
        }

        protected virtual bool ShouldHandleCollisionAsAttack(BaseWeapon attackingWeapon)
        {
            bool isWeaponTarget = attackingWeapon.TargetTags.Contains(transform.tag);
            return attackingWeapon.InUse && HasHp && isWeaponTarget;
        }

        protected virtual void ApplyDamage(BaseWeapon attackingWeapon)
        {
            float damage = attackingWeapon.AttackDamage;
            if (attackingWeapon.isDPSType)
                damage *= Time.deltaTime;

            var newHp = Mathf.Max(CurrentHp - damage, 0f);
            Debug.Log(
                $"Applying damage to {gameObject.name}: currentHp = {CurrentHp}, damage = {damage}, newHp = {newHp}"
            );
            CurrentHp = newHp;
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
