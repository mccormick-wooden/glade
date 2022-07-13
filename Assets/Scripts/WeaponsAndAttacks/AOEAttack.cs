using System;
using System.Linq;
using Assets.Scripts.Abstract;
using UnityEngine;

public class AOEAttack : BaseWeapon
{
    private string[] validCollideTags = new string[] { "Player" };
    DateTime creationTime;
    Collider collider;

    protected override void Start()
    {
        base.Start();
        isDPSType = true;
        TargetTags = new string[] { "Player" };
        creationTime = DateTime.Now;
        InUse = true;
        collider = GetComponent<Collider>();
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
