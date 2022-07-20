using System;
using System.Linq;
using Assets.Scripts.Abstract;
using UnityEngine;

public class PeaWeapon : BaseWeapon
{
    private string[] validCollideTags = new string[] { "Player" };
    DateTime creationTime;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        TargetTags = new string[] { "Player" };
        creationTime = DateTime.Now;
        InUse = true;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (DateTime.Now > creationTime.AddSeconds(2.5))
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
        else
        {
            Destroy(gameObject);
        }
    }
}
