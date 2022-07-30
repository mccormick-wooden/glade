using System;
using Assets.Scripts.Abstract;
using UnityEngine;

public class MushroomExplosionWeapon : BaseWeapon
{
    private string[] validCollideTags = new string[] { "Player" };
    DateTime creationTime;

    [SerializeField]
    AudioClip explodeSound = null;

    [SerializeField]
    float explodePitch;

    [SerializeField]
    float explodeVolume;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        TargetTags = new string[] { "Player" };
        InUse = true;
        creationTime = DateTime.Now;

        EventManager.TriggerEvent<MonsterDieEvent, Vector3, AudioClip, float, float>(transform.position, explodeSound, explodePitch, explodeVolume);
    }

    // Update is called once per frame
    void Update()
    {
        if (DateTime.Now > creationTime.AddSeconds(1))
        {
            Destroy(gameObject);
        }
    }
}
