using Assets.Scripts.Interfaces;
using Assets.Scripts.Damageable;
using UnityEngine;

namespace PlayerBehaviors
{
    [RequireComponent(typeof(Player))]
    public class PlayerDamageable : DisappearDamageable
    {
        protected override bool UseHealthBarText { get; set; } = true;

        protected override void ApplyDamage(IWeapon attackingWeapon, float modifier = 1f)
        {
            base.ApplyDamage(attackingWeapon, 1 / PlayerStats.Instance.DamageResistanceModifier);
        }
    }
}
