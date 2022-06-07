using System.Linq;
using Assets.Scripts.Abstract;
using UnityEngine;

public class Sword : BaseWeapon
{
    private string[] validCollideTags = new string[] { "Enemy", "Damageable" }; // I'm wondering if beacons should have a different tag?

    private void OnCollisionEnter(Collision collision)
    {
        if (!validCollideTags.Contains(collision.gameObject.tag))
        {
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }
}
