﻿using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abstract
{
    public abstract class BaseDamageable : MonoBehaviour, IDamageable
    {
        [SerializeField]
        protected HealthBarController healthBarController;

        [SerializeField]
        private float currentHp;

        [SerializeField]
        private float maxHp;

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

        public virtual bool IsDead { get; set; } = false;

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
            return attackingWeapon != null && attackingWeapon.InUse && HasHp;
        }

        protected virtual void ApplyDamage(BaseWeapon attackingWeapon)
        {
            var newHp = Mathf.Max(CurrentHp - attackingWeapon.AttackDamage, 0f);
            Debug.Log(
                $"Applying damage to {gameObject.name}: currentHp = {CurrentHp}, damage = {attackingWeapon.AttackDamage}, newHp = {newHp}"
            );
            CurrentHp = newHp;
        }

        protected virtual void Die()
        {
            Debug.Log($"{gameObject.name} died.");
            IsDead = true;
        }
    }
}
