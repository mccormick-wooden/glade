using System;
using System.Linq;
using Assets.Scripts.Abstract;
using UnityEngine;

public class AOEAttack : BaseWeapon
{
    private string[] validCollideTags = new string[] { "Player" };
    DateTime creationTime;

    protected override void Start()
    {
        base.Start();
        isDPSType = true;
        TargetTags = new string[] { "Player" };
        creationTime = DateTime.Now;
        InUse = true;
    }

    protected void Update()
    {
        if (DateTime.Now > creationTime.AddSeconds(2.5))
        {
            Destroy(gameObject);
        }
    }

    
}
