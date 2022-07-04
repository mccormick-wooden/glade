using System.Collections.Generic;
using Assets.Scripts.Interfaces;
using UnityEngine;

[RequireComponent(typeof(IDamageable))]
public class CrystalDamageEffect : BaseCrystalEffect
{
    [Tooltip("Nominal damage rate. May be multiplied by stronger crystals.")]
    [SerializeField]
    [Min(0f)]
    private float hpPerSecond;

    private IDamageable health;

    void Awake()
    {
        // Need to be damageable for effect
        Utility.LogErrorIfNull(health = GetComponent<IDamageable>(),
            "IDamageable",
            $"{name} requires some sort of damageable.");
    }

    private void Damage()
    {
        // For each nearby crystal, apply damage
        foreach (KeyValuePair<int, float> crystal in nearbyCrystalIDs)
        {
            Debug.Log($"{name} taking damage from {crystal.Key}");
            float multiplier = crystal.Value;
            CrystalWeapon crystalWeapon = new CrystalWeapon(hpPerSecond * multiplier);
            health.HandleAttack(crystalWeapon);
        }
    }

    protected override void CrystalEffectStart()
    {
        Debug.Log($"{name}: Damage effect active.");
        InvokeRepeating("Damage", 0f, 1f);
    }

    protected override void CrystalEffectStop()
    {
        Debug.Log($"{name}: Damage effect stopping.");
        CancelInvoke("Damage");
    }
}
