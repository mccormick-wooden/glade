using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyBeaconDefenderEnemy : BaseEnemy
{
    [SerializeField]
    public GameObject BeaconToDefend;

    [SerializeField]
    private float maxDistanceFromBeacon;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();

        isAttacking = false;

        attackDelayTimeSeconds = 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttacking)
        {

        }
    }
}
