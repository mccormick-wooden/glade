using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class BaseCrystalEffect : MonoBehaviour
{
    private HashSet<int> nearbyCrystalIDs = new HashSet<int>();

    void Awake()
    {
        // Need to have a collider in order to know when within effect radius
        if (GetComponent<Collider>() == null)
            Debug.LogError("Couldn't find enabled collider component.");
    }

    private void OnDisable()
    {
        CrystalEffectStop();
    }

    private void OnTriggerEnter(Collider other)
    {
        CrystalController crystal;
        if (null != (crystal = other.GetComponent<CrystalController>()))
        {
            nearbyCrystalIDs.Add(crystal.GetInstanceID());
            CrystalEffectStart(crystal.EffectMultiplier);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        CrystalController crystal;
        if (null != (crystal = other.GetComponent<CrystalController>()))
        {
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CrystalController crystal;
        if (null != (crystal = other.GetComponent<CrystalController>()))
        {
            nearbyCrystalIDs.Remove(crystal.GetInstanceID());
            CrystalEffectStop();
        }
    }

    protected virtual void CrystalEffectStart(float multiplier)
    {
        Debug.Log($"{name}: enabling crystal effect.");
    }

    protected virtual void CrystalEffectStop()
    {
        Debug.Log($"{name}: disabling crystal effect.");
    }

}
