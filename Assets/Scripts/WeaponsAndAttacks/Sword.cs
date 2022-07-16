using System.Linq;
using Assets.Scripts.Abstract;
using UnityEngine;

public class Sword : BaseWeapon
{
    private string[] validCollideTags = new string[] { "Enemy", "Damageable" };
    private ParticleSystem trails;

    protected override void Start()
    {
        base.Start();
        TargetTags = new string[] { "Enemy", "Damageable" };  
        trails = transform.Find("Trails").GetComponent<ParticleSystem>();
        trails.Stop();
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (!validCollideTags.Contains(collision.gameObject.tag))
        {
            if (TargetTags.Contains(collision.gameObject.tag))
            {
                Debug.Log("hit a thing!  do trail!");
                trails.Play();
            }

            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (InUse && TargetTags.Contains(other.gameObject.tag))
        {
            Debug.Log("hit a thing!  do trail!");
            trails.Play();
        }
    }
}
