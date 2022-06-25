using Assets.Scripts.Abstract;
using UnityEngine;

[RequireComponent(typeof(BaseDamageable))]
public class CrystalHealer : BaseCrystalEffect
{
    [Tooltip("Nominal healing rate. May be multiplied by stronger crystals.")]
    [SerializeField]
    private float hpPerSecond;

    private float multiplier;
    private BaseDamageable health;

    void Awake()
    {
        // Need to be damageable in order to heal
        if (null == (health = GetComponent<BaseDamageable>()))
            Debug.LogError("Couldn't find damageable component.");
    }

    private void Heal()
    {
        health.Heal(hpPerSecond * multiplier);
    }

    protected override void CrystalEffectStart(float multiplier)
    {
        this.multiplier = multiplier;
        InvokeRepeating("Heal", 0f, 1f);
    }

    protected override void CrystalEffectStop()
    {
        this.multiplier = 1f;
        CancelInvoke("Heal");
    }
}
