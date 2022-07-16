using System.Linq;
using Assets.Scripts.Abstract;
using UnityEngine;

public class Sword : BaseWeapon
{
    private string[] validCollideTags = new string[] { "Enemy", "Damageable" };
    private ParticleSystem trails;

    [SerializeField]
    private GameObject[] hitEffectPrefabs;

    protected override void Start()
    {
        base.Start();
        TargetTags = new string[] { "Enemy", "Damageable" };  
        trails = transform.Find("HitTrails").GetComponent<ParticleSystem>();
        trails.Stop();
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (!validCollideTags.Contains(collision.gameObject.tag))
        {
            if (TargetTags.Contains(collision.gameObject.tag))
            {
                trails.Play();
            }

            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (InUse && TargetTags.Contains(other.gameObject.tag))
        {
            trails.Play();

            int whichHitSound = Random.Range(0, 2);  // yes this should go pull the event manager # of clips, const for now
            EventManager.TriggerEvent<SwordHitEvent, Vector3, int>(transform.position, whichHitSound);

            int whichHitEffect = Random.Range(0, hitEffectPrefabs.Length-1);
            GameObject hitEffect = Instantiate(hitEffectPrefabs[whichHitEffect], other.ClosestPoint(transform.position), Quaternion.identity, null);
            trails.Play();
        }
    }
}
