using System.Collections.Generic;
using Assets.Scripts.Abstract;
using UnityEngine;

[RequireComponent(typeof(BaseDamageable))]
public class CrystalHealEffect : BaseCrystalEffect
{
    [Tooltip("Nominal healing rate. May be multiplied by stronger crystals.")]
    [SerializeField]
    [Min(0f)]
    private float hpPerSecond;

    private BaseDamageable health;

    void Awake()
    {
        // Need to be damageable/healable for healh effect
        Utility.LogErrorIfNull(health = GetComponent<BaseDamageable>(),
            "BaseDamageable",
            $"Requires some sort of damageable such that {name} can be healed.");

        if (null != health && !health.IsHealable)
            Debug.LogError("Attached BaseDamageable must be marked healable.");
    }

    private void Heal()
    {
        // For each nearby crystal, apply healing
        foreach (KeyValuePair<int, float> crystal in nearbyCrystalIDs)
        {
            Debug.Log($"Crystal {crystal.Key} healing");
            float multiplier = crystal.Value;
            health.Heal(hpPerSecond * multiplier);
        }
    }

    protected override void CrystalEffectStart()
    {
        Debug.Log($"{name}: Heal buff active.");
        InvokeRepeating("Heal", 0f, 1f);
    }

    protected override void CrystalEffectStop()
    {
        Debug.Log($"{name}: Heal buff stopping.");
        CancelInvoke("Heal");
    }
}
