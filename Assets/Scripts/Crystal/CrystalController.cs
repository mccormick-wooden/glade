using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SphereCollider))]
public class CrystalController : MonoBehaviour
{
    public float EffectMultiplier = 1;

    [SerializeField]
    private float effectRadius = 5;

    public bool effectActive { get; private set; }

    private SphereCollider effectEnableCollider;

    private Animator anim;
    private int activatorsClose = 0;
    private bool animIsGrowing => anim.GetCurrentAnimatorStateInfo(0).IsName("Growing");

    private void Awake()
    {
        anim = GetComponent<Animator>();
        if (anim == null)
            Debug.Log("Couldn't find animation component");

        // Set the activator collider to the effect radius.
        effectEnableCollider = GetComponent<SphereCollider>();
        if (effectEnableCollider == null)
            Debug.Log("Couldn't find SphereCollider component");
        else
            effectEnableCollider.radius = effectRadius;
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetInteger("ActivatorsClose", activatorsClose);
    }

    private void OnEnable()
    {
        // Let the activators retrigger the crystal
        activatorsClose = 0;
        anim.Play("Growing");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (null != other.GetComponent<BaseCrystalEffect>())
        {
            activatorsClose++;
            Debug.Log($"{name}: Activator arrived. {activatorsClose} in vicinity");
        }
        if (activatorsClose > 0) effectActive = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (null != other.GetComponent<BaseCrystalEffect>())
        {
            activatorsClose--;
            Debug.Log($"{name}: Activator left. {activatorsClose} in vicinity.");
        }
        if (activatorsClose <= 0)
        {
            activatorsClose = 0;
            effectActive = false;
        }
    }
}
