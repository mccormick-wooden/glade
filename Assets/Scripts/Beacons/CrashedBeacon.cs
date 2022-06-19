using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Abstract;
using Beacons;
using UnityEngine;

public class CrashedBeacon : BaseDamageable
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Die()
    {
        base.Die();
        BeaconSpawner.Instance.OnBeaconDeath(this);
    }
}
