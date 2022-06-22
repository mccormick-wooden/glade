using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(Animator))]
public class CrystalController : MonoBehaviour
{
    private Animator anim;
    private int activatorsClose = 0;
    private int animActivatorsClose;

    public bool effectActive { get; private set; }

    [SerializeField]
    private float effectRadius = 5;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        animActivatorsClose = Animator.StringToHash("ActivatorsClose");
        if (anim == null)
        {
            Debug.Log("Couldn't find animation component");
        }
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetInteger(animActivatorsClose, activatorsClose);
        if (effectActive)
        {
            Physics.OverlapSphere(transform.position, effectRadius);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activatorsClose++;
            Debug.Log($"Crystal: Activator arrived. {activatorsClose} in vicinity");
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
