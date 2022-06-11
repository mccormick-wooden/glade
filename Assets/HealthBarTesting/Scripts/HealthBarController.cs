using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace dz
{
    public class HealthBarController : MonoBehaviour
    {
        public Slider hpSlider;

        public float MaxHp
        {
            get => hpSlider.maxValue;
            private set => hpSlider.maxValue = value;
        }

        public float CurrentHp
        {
            get => hpSlider.value;
            private set => hpSlider.value = value;
        }

        // Initializes the HP value.
        public void InitHp(float maxHp)
        {
            MaxHp = maxHp;
            CurrentHp = maxHp;
        }

        public void ApplyHeal(float healBy)
        {
            CurrentHp = Mathf.Min(CurrentHp + healBy, MaxHp);
        }

        public void ApplyDamage(float damageBy)
        {
            CurrentHp = Mathf.Max(CurrentHp - damageBy, 0f);
        }
    }
}

