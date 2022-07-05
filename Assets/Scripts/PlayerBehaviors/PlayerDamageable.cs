using Assets.Scripts.Abstract;
using Assets.Scripts.Damageable;
using UnityEngine;

namespace PlayerBehaviors
{
    [RequireComponent(typeof(Player))]
    public class PlayerDamageable : DisappearDamageable
    {
        protected override void Start()
        {
            base.Start();
            healthBarController.InitHealthBar(CurrentHp, true);
        }

        protected override void ApplyDamage(BaseWeapon attackingWeapon, float modifier = 1f)
        {
            base.ApplyDamage(attackingWeapon, 1 / PlayerStats.Instance.DamageResistanceModifier);
        }
    }
}
