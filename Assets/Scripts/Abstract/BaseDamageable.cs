using Assets.Scripts.Interfaces;
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

        public virtual float CurrentHp
        {
            get => currentHp;
            protected set => currentHp = value;
        }

        public virtual float MaxHp
        {
            get => maxHp;
            protected set => maxHp = value;
        }

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

        /// <summary>
        /// This method should be directly called or overriden by derived implementations in collision handlers.
        /// </summary>
        /// <param name="damagingWeapon"></param>
        protected virtual void HandleAttack(BaseWeapon attackingWeapon)
        {
            ApplyDamage(attackingWeapon);

            if (healthBarController != null)
                healthBarController.CurrentHp = CurrentHp;

            if (CurrentHp == 0)
                Die();
        }

        protected virtual bool ShouldHandleCollisionAsAttack(Collider other)
        {
            if (CurrentHp > 0)
            {
                var attackingWeapon = other.GetComponent<BaseWeapon>();
                return attackingWeapon != null && attackingWeapon.InUse;
            }

            return false;
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
            gameObject.SetActive(false);
        }
    }
}
