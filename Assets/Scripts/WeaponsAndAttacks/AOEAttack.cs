using System;
using Assets.Scripts.Abstract;
using UnityEngine;

public class AOEAttack : BaseWeapon
{
    private string[] validCollideTags = new string[] { "Player" };
    DateTime creationTime;
    private new Collider collider;

    protected override void Start()
    {
        base.Start();
        isDPSType = true;
        TargetTags = new string[] { "Player" };
        creationTime = DateTime.Now;
        InUse = true;
        collider = GetComponent<Collider>();

        EventManager.TriggerEvent<FairyAOEAttackEvent, Vector3>(transform.position);
    }

    protected void Update()
    {
        if (DateTime.Now > creationTime.AddSeconds(3.0))
        {
            Destroy(gameObject);
        }
        else if (DateTime.Now > creationTime.AddSeconds(1.5))
        {
            transform.Find("Cylinder").gameObject.SetActive(true);
            collider.enabled = true;
        }
    }

    
}
