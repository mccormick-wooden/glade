using Assets.Scripts.Interfaces;
using Assets.Scripts.Damageable;
using UnityEngine;

namespace PlayerBehaviors
{
    [RequireComponent(typeof(Player))]
    public class PlayerDamageable : DisappearDamageable
    {
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        protected override void Start()
        {
            UseHealthBarText = true;
            base.Start();
            IsHealable = true;
        }

        public override void HandleAttack(IWeapon attackingWeapon)
        {
            animator.CrossFade("Head Back From Hit", 0.2f);
            EventManager.TriggerEvent<PlayerHurtEvent, Vector3>(transform.position);

            base.HandleAttack(attackingWeapon);
        }

        protected override void ApplyDamage(IWeapon attackingWeapon, float modifier = 1f)
        {
            base.ApplyDamage(attackingWeapon, 1 / PlayerStats.Instance.DamageResistanceModifier);
        }
    }
}
