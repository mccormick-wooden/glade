using System;
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
        }

        public override void HandleAttack(IWeapon attackingWeapon)
        {
            animator.CrossFade("Knocked Back from Hit", 0.2f);
            base.HandleAttack(attackingWeapon);
        }

        protected override void ApplyDamage(IWeapon attackingWeapon, float modifier = 1f)
        {
            base.ApplyDamage(attackingWeapon, 1 / PlayerStats.Instance.DamageResistanceModifier);
        }
    }
}
