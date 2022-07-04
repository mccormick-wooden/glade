using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpirit : MonoBehaviour
{
    /// <summary>
    /// The playerGameObjectRootName must be the name of the physical player model.
    /// </summary>
    [SerializeField]
    private string playerModelGameObjectRootName = "PlayerModel";

    private GameObject playerModel;
    private Animator animator;
    
    private void Awake()
    {
        playerModel = GameObject.Find(playerModelGameObjectRootName);
        Utility.LogErrorIfNull(playerModel, nameof(playerModel));

        animator = GetComponent<Animator>();
        Utility.LogErrorIfNull(animator, nameof(animator));
    }

    private void Update()
    {
        transform.LookAt(playerModel.transform); // TODO: If time, consider adding animation
        //var lookRotation = Quaternion.LookRotation((playerModel.transform.position - transform.position).normalized);

        //transform.rotation = lookRotation;

        //if (lookRotation.x > 0)
        //{
        //    animator.SetTrigger("Move");
        //}
        //else
        //{
        //    animator.SetTrigger("Idle");
        //}
    }
}
