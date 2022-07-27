using System;

namespace Assets.Scripts.Interfaces
{
    public interface IDamageable
    {
        // Perhaps also should enforce a collider and rigidbody?
        float MaxHp { get; }
        float CurrentHp { get; }
        bool HasHp { get; }
        bool IsDead { get; }
        bool IsHealable { get; set; }
        bool IsImmune { get; set; }
        void Heal(float healAmount);
        void HandleAttack(IWeapon weapon);
        bool enabled { get; set; }
        Action<IDamageable, string, int> Died { get; set; }
    }
}
