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

    void BeConsumed(Transform consumer)
    {
        IDamageable damageable = consumer.GetComponent<IDamageable>();
        if (damageable != null)
            damageable.Heal(25);

        Instantiate(healParticleEffect, consumer);
    }
}
