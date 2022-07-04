using Assets.Scripts.Interfaces;
using System;
using System.Collections.Generic;
using Assets.Scripts.Abstract;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class BaseCrystalEffect : MonoBehaviour
{
    protected Dictionary<int, float> nearbyCrystalIDs = new Dictionary<int, float>();

    private bool effectActive;

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

    private void OnTriggerEnter(Collider other)
    {
        CrystalController crystal;
        if (!effectActive && (null != (crystal = other.GetComponent<CrystalController>())))
        {
            // If it's a damageable crystal, add a callback function for when it dies
            BaseDamageable crystalDamageable = other.GetComponent<BaseDamageable>();
            if (crystalDamageable != null)
                crystalDamageable.Died += OnDiedCrystalRemoveEffect;

            // Add its ID to our list of nearby crystals
            AddCrystalEffect(crystal.CrystalID, crystal.EffectMultiplier);

            // Start the effect, but only if it isn't already started
            if (!effectActive) BaseCrystalEffectStart();
        }
    }

    private void OnDiedCrystalRemoveEffect(IDamageable damageable, string name, int crystalID)
    {
        Debug.Log($"{name}: Nearby Crystal {crystalID} died.");
        damageable.Died -= OnDiedCrystalRemoveEffect;
        RemoveCrystalEffect(crystalID);
    }

    private void OnTriggerExit(Collider other)
    {
        CrystalController crystal;
        if (null != (crystal = other.GetComponent<CrystalController>()))
        {
            RemoveCrystalEffect(crystal.CrystalID);
        }
    }

    protected void AddCrystalEffect(int crystalID, float multiplier)
    {
        Debug.Log($"{name}: Adding Crystal {crystalID} effect.");
        nearbyCrystalIDs.Add(crystalID, multiplier);
    }

    protected void RemoveCrystalEffect(int crystalID)
    {
        Debug.Log($"{name}: Removing Crystal {crystalID} effect.");
        nearbyCrystalIDs.Remove(crystalID);

        // If we have no more nearby crystals, stop the effect
        if (nearbyCrystalIDs.Count <= 0)
            BaseCrystalEffectStop();
    }

    protected abstract void CrystalEffectStart();

    protected abstract void CrystalEffectStop();

    private void BaseCrystalEffectStart()
    {
        effectActive = true;
        CrystalEffectStart();
    }

    private void BaseCrystalEffectStop()
    {
        // Just in case, clear out the nearby crystal IDs dictionary
        nearbyCrystalIDs.Clear();

        effectActive = false;
        CrystalEffectStop();
    }

}
