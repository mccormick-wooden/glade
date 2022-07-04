﻿using System;

namespace Assets.Scripts.Interfaces
{
    public interface IDamageable
    {
        // Perhaps also should enforce a collider and rigidbody?
        float MaxHp { get; }
        float CurrentHp { get; }
        bool HasHp { get; }
        bool IsDead { get; }
        bool IsHealable { get; }
        public void Heal(float healAmount);
        Action<IDamageable, string, int> Died { get; set; }
    }
}
