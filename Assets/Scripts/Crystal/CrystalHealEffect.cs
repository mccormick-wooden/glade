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
        // Need to be damageable in order to heal
        Utility.LogErrorIfNull(health = GetComponent<BaseDamageable>(), "BaseDamageable", $"Requires some sort of damageable such that {name} can be healed.");
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
        base.CrystalEffectStart();
        Debug.Log($"{name}: starting heal effect.");
        InvokeRepeating("Heal", 0f, 1f);
    }

    protected override void CrystalEffectStop()
    {
        base.CrystalEffectStop();
        Debug.Log($"{name}: canceling heal effect.");
        CancelInvoke("Heal");
    }
}
