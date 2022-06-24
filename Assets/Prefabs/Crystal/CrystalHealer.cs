using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Abstract;
using UnityEngine;

[RequireComponent(typeof(BaseDamageable))]
public class CrystalHealer : MonoBehaviour
{
    [SerializeField]
    private float hpPerSecond;

    private BaseDamageable health;

    void Awake()
    {
        if (null == (health = GetComponent<BaseDamageable>()))
            Debug.LogError("Couldn't find damageable component.");
    }

    void OnEnable()
    {
        EventManager.StartListening<CrystalEffectEvent, Vector3, float, float>(CrystalEffectEventHandler);
    }

    private void OnDisable()
    {
        EventManager.StopListening<CrystalEffectEvent, Vector3, float, float>(CrystalEffectEventHandler);
    }

    private void CrystalEffectEventHandler(Vector3 position, float effectRadius, float effectMultiplier)
    {
        var distance = (position - transform.position).magnitude;
        if (distance <= effectRadius)
            health.Heal(hpPerSecond * effectMultiplier);
    }
}
