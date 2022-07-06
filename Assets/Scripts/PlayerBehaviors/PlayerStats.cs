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

        public void UpdateMaxHealthModifier(float summand)
        {
            MaxHealthModifier += summand;
            
            // The scalar in UpdateMaxHealth is applied on top of any other existing benefits, so we don't want to apply the growing modifier from the stats file
            // In the future the max health should have a base value that derives it's 'real' value using the modifier
            _playerDamageable.UpdateMaxHealth(1 + summand);
        }

        public void UpdateDamageModifier(float summand)
        {
            DamageModifier += summand;

            /*
             * While this works for the current state of the game it's obviously not perfect as it only applies to a sword
             * Combat work is planned so where the code should go to actually affect damage output is likely to change.
             */
            if (_sword == null) return;
            
            var oldAttackDamage = _sword.AttackDamage;
            // Again, no base attack damage to store the baseline so we're just creating a scalar from the summand
            _sword.AttackDamage = oldAttackDamage * (1 + summand); 
        }

        public void UpdateDamageResistanceModifier(float summand)
        {
            DamageResistanceModifier += summand;
        }
    }
}
