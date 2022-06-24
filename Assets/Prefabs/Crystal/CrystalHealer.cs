using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Abstract;
using UnityEngine;

[RequireComponent(typeof(BaseDamageable))]
[RequireComponent(typeof(Collider))]
public class CrystalEffect : MonoBehaviour
{
    [SerializeField]
    private GameObject crystal;

    [SerializeField]
    private float hpPerSecond;

    private Collider myCollider;
    private BaseDamageable health;

    // Start is called before the first frame update
    void Awake()
    {
        if (null == (myCollider = GetComponent<Collider>()))
            Debug.LogError("Couldn't find collider.");

        if (null == (health = GetComponent<BaseDamageable>()))
            Debug.LogError("Couldn't find damageable.");
    }

    private void Start()
    {
        crystal.GetComponent<CrystalController>().RegisterForCrystalEffect(myCollider.GetInstanceID(), Heal);
    }

    public void Heal()
    {
        health.Heal(hpPerSecond*Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
