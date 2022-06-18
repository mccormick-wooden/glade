using System;
using System.Linq;
using Assets.Scripts.Abstract;
using UnityEngine;

public class Arrow : BaseWeapon
{
    private string[] validCollideTags = new string[] { "Player" }; // I'm wondering if beacons should have a different tag?
    DateTime creationTime;

    protected virtual void Start()
    {
        TargetTags = new string[] { "Player" };
        creationTime = DateTime.Now;
    }

    protected void Update()
    {
        if (DateTime.Now > creationTime + TimeSpan.FromSeconds(2.5))
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!validCollideTags.Contains(collision.gameObject.tag))
        {
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }
}
