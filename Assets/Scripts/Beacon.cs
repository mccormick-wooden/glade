using Assets.Scripts.Abstract;
using TMPro;
using UnityEngine;

public class Beacon : BaseDamageable
{
    public TextMeshProUGUI beaconCountText;

    void DecrementBeaconCount()
    {
        SetBeaconCountText(false);
    }

    void IncrementBeaconCount()
    {
        SetBeaconCountText(true);
    }

    void OnDisable()
    {
        DecrementBeaconCount();
    }

    void SetBeaconCountText(bool increment)
    {
        int newCount;

        string oldCount = beaconCountText.text.Replace("Beacons: ", null);
        if (int.TryParse(oldCount, out newCount))
        {
            newCount = increment ? ++newCount : --newCount;
            beaconCountText.text = string.Format("Beacons: {0}", newCount >= 0 ? newCount : 0);
        }
        else
        {
            Debug.LogError("Could not parse int from beaconCountText", beaconCountText);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ShouldHandleCollisionAsAttack(other))
            HandleAttack(other.GetComponent<BaseWeapon>()); // entirely base behavior
    }

    protected override void Start()
    {
        base.Start();
        IncrementBeaconCount();
    }
}
