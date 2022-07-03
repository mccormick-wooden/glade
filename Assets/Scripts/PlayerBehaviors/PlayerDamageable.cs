using Assets.Scripts.Abstract;
using Assets.Scripts.Damageable;
using UnityEngine;

namespace PlayerBehaviors
{
    [RequireComponent(typeof(Player))]
    public class PlayerDamageable : DisappearDamageable
    {
        public void UpdateMaxHealth(float scalar)
        {
            var oldMaxHp = MaxHp;
            var newMaxHp = Mathf.Floor(oldMaxHp * scalar);
            MaxHp = newMaxHp < 1 ? 1 : newMaxHp;

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

            Debug.Log("Changed Current HP from " + oldCurrentHp + " to " + CurrentHp);
        }

        protected override void ApplyDamage(BaseWeapon attackingWeapon, float modifier = 1f)
        {
            base.ApplyDamage(attackingWeapon, 1 / PlayerStats.Instance.DamageResistanceModifier);
        }
    }
}
