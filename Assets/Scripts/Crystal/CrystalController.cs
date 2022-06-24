using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SphereCollider))]
public class CrystalController : MonoBehaviour
{
    [SerializeField]
    private float effectRadius = 5;
    [SerializeField]
    private float effectMultiplier = 1;

    public bool effectActive { get; private set; }

    private SphereCollider effectEnableCollider;

    private HashSet<int> ActivatorInstanceIds = new HashSet<int>();

    private Animator anim;
    private int activatorsClose = 0;
    private bool animIsGrowing => anim.GetCurrentAnimatorStateInfo(0).IsName("Growing");

    private float nextEffectTime;

    /// <summary>
    /// Registers a collider as something that can activate the crystal. I
    /// wanted to avoid using tags. There's probably a more C# or Unity way of
    /// doing this, but this works.
    /// </summary>
    public void RegisterActivator(Collider collider)
    {
        Debug.Log($"{name} registering activator {collider.GetInstanceID()}");
        ActivatorInstanceIds.Add(collider.GetInstanceID());
    }

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

        // Set the first effect time to be one second in the future.
        nextEffectTime = Mathf.Floor(Time.time) + 1;
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetInteger("ActivatorsClose", activatorsClose);

        // Since we know when a crystal is active, might as well avoid every
        // crystal spamming events while they're unactivated.
        // Also, only trigger the crystal effect once per second.
        if (effectActive && (Time.time > nextEffectTime))
        {
            EventManager.TriggerEvent<CrystalEffectEvent, Vector3, float, float>(transform.position, effectRadius, effectMultiplier);
        }
        nextEffectTime = Mathf.Floor(Time.time) + 1;
    }

    private void OnEnable()
    {
        // Let the activators retrigger the crystal
        activatorsClose = 0;
        anim.Play("Growing");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ActivatorInstanceIds.Contains(other.GetInstanceID()))
        {
            activatorsClose++;
            Debug.Log($"{name}: Activator arrived. {activatorsClose} in vicinity");
        }
        if (activatorsClose > 0) effectActive = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (ActivatorInstanceIds.Contains(other.GetInstanceID()))
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
