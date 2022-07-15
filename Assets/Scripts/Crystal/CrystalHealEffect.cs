using System.Collections.Generic;
using Assets.Scripts.Interfaces;
using UnityEngine;

[RequireComponent(typeof(IDamageable))]
public class CrystalHealEffect : BaseCrystalEffect
{
    [Tooltip("Nominal healing rate. May be multiplied by stronger crystals.")]
    [SerializeField]
    [Min(0f)]
    private float hpPerSecond;

    private IDamageable health;

    void Awake()
    {
        // Need to be damageable/healable for healh effect
        Utility.LogErrorIfNull(health = GetComponent<IDamageable>(),
            "IDamageable",
            $"Requires some sort of damageable such that {name} can be healed.");

        if (health != null)
            health.IsHealable = true;
    }

    private void Heal()
    {
        // For each nearby crystal, apply healing
        foreach (KeyValuePair<int, float> crystal in nearbyCrystalIDs)
        {
            float multiplier = crystal.Value;
            health.Heal(hpPerSecond * multiplier);
        }
    }

    protected override void CrystalEffectStart()
    {
        Debug.Log($"{name}: Crystal heal buff active.");
        InvokeRepeating("Heal", 0f, 1f);
    }

    protected override void CrystalEffectStop()
    {
        Debug.Log($"{name}: Crystal heal buff stopping.");
        CancelInvoke("Heal");
    }
}
