using System.Collections.Generic;
using UnityEngine;

namespace PlayerBehaviors
{
    [RequireComponent(typeof(Player))]
    public class PlayerStats : MonoBehaviour
    {
        public static PlayerStats Instance { get; private set; }

        public float MaxHealthModifier { get; private set; }
        public float DamageModifier { get; private set; }
        public float DamageResistanceModifier { get; private set; }

        private readonly List<BasePowerUp> powerUps = new List<BasePowerUp>();

        // These are the components we need to access to give the power ups an effect
        private PlayerDamageable _playerDamageable;
        private Sword _sword;

        private void Awake()
        {
            Instance = this;
            MaxHealthModifier = 1f;
            DamageModifier = 1f;
            DamageResistanceModifier = 1f;
            _playerDamageable = GetComponent<PlayerDamageable>();
            _sword = GetComponentInChildren<Sword>();
        }

        public void AddToPowerUps(BasePowerUp newPowerUp)
        {
            powerUps.Add(newPowerUp);
            Debug.Log("Player now has " + powerUps.Count + " power up(s).");
        }

        public void ScaleMaxHealthModifier(float scalar)
        {
            MaxHealthModifier *= scalar;
            _playerDamageable.UpdateMaxHealth(MaxHealthModifier);
        }

        public void ScaleDamageModifier(float scalar)
        {
            DamageModifier *= scalar;

            /*
             * While this works for the current state of the game it's obviously not perfect as it only applies to a sword
             * Combat work is planned so where the code should go to actually affect damage output is likely to change.
             */
            if (_sword == null) return;

            var oldAttackDamage = _sword.AttackDamage;
            _sword.AttackDamage = oldAttackDamage * DamageModifier;
        }

        public void ScaleDamageResistanceModifier(float scalar)
        {
            DamageResistanceModifier *= scalar;
        }
    }
}
