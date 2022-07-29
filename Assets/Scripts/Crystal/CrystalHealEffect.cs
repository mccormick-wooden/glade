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

    public GameObject healParticlePrefab;
    private HealingParticles aura;

    void Awake()
    {
        // Need to be damageable/healable for healh effect
        Utility.LogErrorIfNull(health = GetComponent<IDamageable>(),
            "IDamageable",
            $"Requires some sort of damageable such that {name} can be healed.");

        if (health != null)
            health.IsHealable = true;

        if (healParticlePrefab != null)
        {
            GameObject healParticles = Instantiate(healParticlePrefab, transform);
            healParticles.name = $"{name}HealAura";
            aura = healParticles.GetComponent<HealingParticles>();
        }
    }

    protected override void CrystalEffectUpdate()
    {
        if (aura != null)
        {
            if (!aura.Active)
            {
                if (!health.IsDead && isActiveAndEnabled && EffectActive && health.CurrentHp < health.MaxHp)
                    aura.EffectStart();
            }
            else
            {
                if (!isActiveAndEnabled || !EffectActive || health.IsDead || health.CurrentHp >= health.MaxHp)
                    aura.EffectStop();
            }
        }
    }

    private void Heal()
    {
        // For each nearby crystal, apply healing
        foreach (KeyValuePair<string, float> crystal in nearbyCrystals)
        {
            GameObject thisCrystal = GameObject.Find(crystal.Key);
            if (thisCrystal != null && thisCrystal.activeInHierarchy && health.CurrentHp <= health.MaxHp)
            {
                float multiplier = crystal.Value;
                health.Heal(hpPerSecond * multiplier);
            }
        }
    }

    private void OnDisable()
    {
        aura.EffectStop();
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
