using System;
using System.Collections.Generic;
using Assets.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class GladeHealthManager : MonoBehaviour
{
    protected enum GladeHealthState
    {
        High,
        Med,
        Low
    }

    public Action GladeDied;

    public bool GladeIsDead => gladeHealthBar != null && gladeHealthBar.value <= gladeHealthBar.minValue;

    private Slider gladeHealthBar;

    private List<IDamageable> activeEnemies = new List<IDamageable>(100);

    protected Dictionary<GladeHealthState, Image> gladeHealthImageDict = new Dictionary<GladeHealthState, Image>();

    public float CurrentHP => gladeHealthBar.value;
    public float MaxHP => gladeHealthBar.maxValue;

    /// <summary>
    /// Higher values result in slower health reduction
    /// </summary>
    [SerializeField]
    protected float gladeHealthReductionFactor = 3f;

    protected float GladeHealthReduction => activeEnemies.Count / gladeHealthReductionFactor;

    protected float GladeHealthHealthIncrease => 10f;

    private void Start()
    {
        #region gladehealth
        gladeHealthBar = GameObject.Find("HUDGladeHealthBar").GetComponent<Slider>();
        Utility.LogErrorIfNull(gladeHealthBar, nameof(gladeHealthBar));

        gladeHealthImageDict = new Dictionary<GladeHealthState, Image>
        {
            { GladeHealthState.High, GameObject.Find("HighGladeHealthFill").GetComponent<Image>() },
            { GladeHealthState.Med, GameObject.Find("MedGladeHealthFill").GetComponent<Image>() },
            { GladeHealthState.Low, GameObject.Find("LowGladeHealthFill").GetComponent<Image>() }
        };

        UpdateGladeHealth(health: 100);

        InvokeRepeating("GladeHealthDecrementer", 1f, 0.5f);
        #endregion
    }

    public void BeaconDied()
    {
        UpdateGladeHealth(gladeHealthBar.value + GladeHealthHealthIncrease);
    }

    public void OnEnemySpawned(IDamageable damageModel)
    {
        activeEnemies.Add(damageModel);
    }

    public void OnEnemyDied(IDamageable damageModel)
    {
        activeEnemies.Remove(damageModel);
    }

    private void GladeHealthDecrementer()
    {
        UpdateGladeHealth(gladeHealthBar.value - GladeHealthReduction);
    }

    public void UpdateGladeHealth(float health)
    {
        if (GladeIsDead)
            return;

        GladeHealthState state;

        if (health > 66)
            state = GladeHealthState.High;
        else if (health > 33)
            state = GladeHealthState.Med;
        else
            state = GladeHealthState.Low;

        foreach (var kvp in gladeHealthImageDict)
        {
            kvp.Value.enabled = kvp.Key == state;
        }

        gladeHealthBar.fillRect = gladeHealthImageDict[state].rectTransform;

        gladeHealthBar.value = health;

        if (GladeIsDead)
        {
            GladeDied?.Invoke();
            CancelInvoke();
        }
    }
}
