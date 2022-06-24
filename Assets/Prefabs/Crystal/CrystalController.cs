using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public delegate void CrystalEffectCallback();

[RequireComponent(typeof(Animator))]
public class CrystalController : MonoBehaviour
{
    private Animator anim;
    private int activatorsClose = 0;
    private int animActivatorsClose;

    public bool effectActive { get; private set; }

    private Dictionary<int, CrystalEffectCallback> registrations = new Dictionary<int, CrystalEffectCallback>();

    [SerializeField]
    private float effectRadius = 5;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        animActivatorsClose = Animator.StringToHash("ActivatorsClose");
        if (anim == null)
            Debug.Log("Couldn't find animation component");
    }

    public void RegisterForCrystalEffect(int instanceId, CrystalEffectCallback callback)
    {
        registrations.Add(instanceId, callback);
    }

    public void Unregister(int instanceId) { registrations.Remove(instanceId); }

    // Update is called once per frame
    void Update()
    {
        anim.SetInteger(animActivatorsClose, activatorsClose);
        if (effectActive)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, effectRadius);
            foreach (var hitCollider in hitColliders)
            {
                int instanceId = hitCollider.GetInstanceID();
                if (registrations.ContainsKey(instanceId))
                    registrations[instanceId]();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activatorsClose++;
            Debug.Log($"Crystal: Activator {other.GetInstanceID()} arrived. {activatorsClose} in vicinity");
        }
        if (activatorsClose > 0) effectActive = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activatorsClose--;
            Debug.Log($"Crystal: Activator left. {activatorsClose} in vicinity.");
        }
        if (activatorsClose <= 0) effectActive = false;
    }
}
