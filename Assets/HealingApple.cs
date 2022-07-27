using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Interfaces;
using UnityEngine;

public class HealingApple : MonoBehaviour
{
    [SerializeField]
    ParticleSystem healParticleEffect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BeConsumed(Transform consumer)
    {
        IDamageable damageable = consumer.GetComponent<IDamageable>();
        if (damageable != null)
        {
            Instantiate(healParticleEffect, consumer);
            damageable.Heal(25);
            EventManager.TriggerEvent<PlayerEatAppleEvent, Vector3>(transform.position);
            Destroy(gameObject);
        }

    }

    public void OnCollisionEnter(Collision collision)
    {
        const int TERRAIN_LAYER_ID = 7;
        if (collision.gameObject.layer == TERRAIN_LAYER_ID)
        {
            EventManager.TriggerEvent<AppleHitGrassEvent, Vector3>(transform.position);
        }
    }

}
