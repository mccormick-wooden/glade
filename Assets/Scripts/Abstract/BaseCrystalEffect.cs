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
        CrystalEffectStop();
    }

    private void OnTriggerEnter(Collider other)
    {
        CrystalController crystal;
        if (null != (crystal = other.GetComponent<CrystalController>()))
        {
            int crystalID = other.GetInstanceID();

            // If it's a damageable crystal, add a callback function for when it dies
            BaseDamageable crystalDamageable = other.GetComponent<BaseDamageable>();
            if (crystalDamageable != null)
                crystalDamageable.Died += (IDamageable, name, instanceID) => { RemoveCrystalEffect(crystalID); };

            // Add its ID to our list of nearby crystals
            AddCrystalEffect(crystalID, crystal.EffectMultiplier);

            // Start the effect, but only if it isn't already started
            if (!effectActive) CrystalEffectStart();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CrystalController crystal;
        if (null != (crystal = other.GetComponent<CrystalController>()))
        {
            RemoveCrystalEffect(other.GetInstanceID());
        }
    }

    protected void AddCrystalEffect(int crystalID, float multiplier)
    {
        Debug.Log($"{name}: Crystal {crystalID} is nearby");
        nearbyCrystalIDs.Add(crystalID, multiplier);
    }

    protected void RemoveCrystalEffect(int crystalID)
    {
        Debug.Log($"{name}: Crystal {crystalID} no longer in range");
        nearbyCrystalIDs.Remove(crystalID);

        // If we have no more nearby crystals, stop the effect
        if (nearbyCrystalIDs.Count <= 0)
            CrystalEffectStop();
    }

    protected virtual void CrystalEffectStart()
    {
        effectActive = true;
    }

    protected virtual void CrystalEffectStop()
    {
        // Just in case, clear out the nearby crystal IDs dictionary
        nearbyCrystalIDs.Clear();
        effectActive = false;
    }

}
