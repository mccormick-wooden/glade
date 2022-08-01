using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Abstract;
using Assets.Scripts.Interfaces;
using UnityEngine;

public class HealingApple : MonoBehaviour
{
    [SerializeField]
    ParticleSystem healParticleEffect;

    public Action<HealingApple> AppleEaten;

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
        BaseDamageable damageable = consumer.GetComponent<BaseDamageable>();
        if (damageable != null)
        {
            Instantiate(healParticleEffect, consumer);
            damageable.Heal(100);
            EventManager.TriggerEvent<PlayerEatAppleEvent, Vector3>(transform.position);
            AppleEaten?.Invoke(this);
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
