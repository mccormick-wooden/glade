using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Abstract;
using UnityEngine;

public class FrontMover : SpecialEffectWeapon
{
    public Transform pivot;
    public ParticleSystem effect;
    public float speed = 15f;
    public float drug = 1f;
    public float repeatingTime = 1f;

    private float startSpeed = 0f;

    protected override void Start()
    {
        base.Start();
        baseAttackDamage = 10f;
        currentAttackDamage = baseAttackDamage;
        InvokeRepeating(nameof(StartAgain), 0f, repeatingTime);
        effect.Play();
        startSpeed = speed;
    }

    private void StartAgain()
    {
        startSpeed = speed;
        transform.position = pivot.position;
    }

    private void Update()
    {
        startSpeed = startSpeed * drug;
        transform.position += transform.forward * (startSpeed * Time.deltaTime);
    }
}
