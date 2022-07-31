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

    public GameObject LightningTarget;

    void Awake()
    {
        if (LightningTarget == null) LightningTarget = gameObject;

        // Need to be damageable for effect
        Utility.LogErrorIfNull(health = GetComponent<IDamageable>(),
            "IDamageable",
            $"{name} requires some sort of damageable.");
    }

    private void Damage()
    {
        // For each nearby crystal, apply damage
        foreach (KeyValuePair<string, float> crystal in nearbyCrystals)
        {
            GameObject thisCrystal = GameObject.Find(crystal.Key);
            if (thisCrystal != null && thisCrystal.activeInHierarchy && thisCrystal.GetComponent<CrystalController>().IsZappingMe(gameObject))
            {
                float multiplier = crystal.Value;
                CrystalWeapon crystalWeapon = new CrystalWeapon(hpPerSecond * multiplier);
                health.HandleAttack(crystalWeapon);
            }
        }
    }

    protected override void CrystalEffectStart()
    {
        if (debugOutput)
            Debug.Log($"{name}: Crystal damage effect starting.");
        InvokeRepeating("Damage", 0f, 1f);
    }

    protected override void CrystalEffectStop()
    {
        if (debugOutput)
            Debug.Log($"{name}: Crystal damage effect stopping.");
        CancelInvoke("Damage");
    }
}
