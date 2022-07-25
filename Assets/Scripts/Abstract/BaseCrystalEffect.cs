using Assets.Scripts.Interfaces;
using System;
using System.Collections.Generic;
using Assets.Scripts.Abstract;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class BaseCrystalEffect : MonoBehaviour
{
    protected Dictionary<string, float> nearbyCrystals = new Dictionary<string, float>();

    private bool effectActive;

    [SerializeField]
    protected bool debugOutput = true;
    
    public bool EffectActive { get => effectActive; }

    void Awake()
    {
        // Need to have a collider in order to know when within effect radius
        Utility.LogErrorIfNull(GetComponent<Collider>(), "Collider", "Collider required for detecting crystals.");
    }

    private void OnDisable()
    {
        // Stop the crystal effect so if GameObject is reenabled, it will start
        // clean
        BaseCrystalEffectStop();
    }

    protected void Update()
    {
        if (effectActive)
        {
            if (nearbyCrystals.Count <= 0 || !isActiveAndEnabled)
                BaseCrystalEffectStop();
        }
        else
        {
            if (nearbyCrystals.Count > 0 && isActiveAndEnabled)
                BaseCrystalEffectStart();
        }

        CrystalEffectUpdate();
    }

    private void OnTriggerEnter(Collider other)
    {
        CrystalController crystal;
        if (null != (crystal = (other.GetComponent<CrystalController>())))
        {
            // Add its ID to our list of nearby crystals
            AddNearbyCrystal(crystal);
        }
    }

    private void OnDiedRemoveCrystal(IDamageable damageable, string crystalName, int crystalID)
    {
        if (debugOutput)
          Debug.Log($"{name}: Nearby Crystal {crystalName} died.");
        damageable.Died -= OnDiedRemoveCrystal;
        RemoveNearbyCrystal(crystalName);
    }

    private void OnTriggerExit(Collider other)
    {
        CrystalController crystal;
        if (null != (crystal = other.GetComponent<CrystalController>()))
        {
            RemoveNearbyCrystal(crystal.name);
        }
    }

    protected void AddNearbyCrystal(CrystalController crystal)
    {
        if (!nearbyCrystals.ContainsKey(crystal.name))
        {
            nearbyCrystals[crystal.name] = crystal.EffectMultiplier;

            // If it's a damageable crystal, add a callback function to remove it when it dies
            BaseDamageable crystalDamageable = crystal.GetComponentInChildren<BaseDamageable>();
            if (crystalDamageable != null)
            {
                Debug.Log($"Subscribing to {crystalDamageable.AttachedName} death");
                crystalDamageable.Died += OnDiedRemoveCrystal;
            }
        }
    }

    protected void RemoveNearbyCrystal(string name)
    {
        nearbyCrystals.Remove(name);
    }

    protected virtual void CrystalEffectUpdate() { }
    protected abstract void CrystalEffectStart();
    protected abstract void CrystalEffectStop();

    private void BaseCrystalEffectStart()
    {
        effectActive = true;
        CrystalEffectStart();
    }

    private void BaseCrystalEffectStop()
    {
        effectActive = false;
        CrystalEffectStop();
    }

}
