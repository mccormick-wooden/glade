using System.Linq;
using Assets.Scripts.Abstract;
using UnityEngine;

public class Sword : BaseWeapon
{
    private string[] validCollideTags = new string[] { "Enemy", "Damageable" };

    protected override void Start()
    {
        base.Start();
        TargetTags = new string[] { "Enemy", "Damageable" };  
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (!validCollideTags.Contains(collision.gameObject.tag))
        {
            if (TargetTags.Contains(collision.gameObject.tag))
            {

            }

            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }
}
