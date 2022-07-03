using System.Collections.Generic;
using UnityEngine;

namespace PlayerBehaviors
{
    public class PlayerStats : MonoBehaviour
    {
        public static PlayerStats Instance { get; private set; }

        public float maxHealthModifier = 1f;
        public float damageModifier = 1f;
        public float damageResistanceModifier = 1f;

        private readonly List<BasePowerUp> powerUps = new List<BasePowerUp>();

        private void Awake()
        {
            Instance = this;
        }

        public void AddToPowerUps(BasePowerUp newPowerUp)
        {
            powerUps.Add(newPowerUp);
            Debug.Log("Player now has " + powerUps.Count + " power up(s).");
        }
    }
}
