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

    private Animator anim;
    private int activatorsClose = 0;
    private int animActivatorsClose;

    private float nextEffectTime;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        animActivatorsClose = Animator.StringToHash("ActivatorsClose");
        if (anim == null)
            Debug.Log("Couldn't find animation component");

        effectEnableCollider = GetComponent<SphereCollider>();
        if (effectEnableCollider == null)
            Debug.Log("Couldn't find SphereCollider component");
        else
            effectEnableCollider.radius = effectRadius;

        nextEffectTime = Mathf.Floor(Time.time) + 1;
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetInteger(animActivatorsClose, activatorsClose);
        if (effectActive && (Time.time > nextEffectTime))
        {
            EventManager.TriggerEvent<CrystalEffectEvent, Vector3, float, float>(transform.position, effectRadius, effectMultiplier);
        }
        nextEffectTime = Mathf.Floor(Time.time) + 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activatorsClose++;
            Debug.Log($"{name}: Activator arrived. {activatorsClose} in vicinity");
        }
        if (activatorsClose > 0) effectActive = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
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
