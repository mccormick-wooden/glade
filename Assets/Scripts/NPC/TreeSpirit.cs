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
    private GameObject ent;
    private Animator animator;
    
    private void Awake()
    {
        playerModel = GameObject.Find(playerModelGameObjectRootName);
        Utility.LogErrorIfNull(playerModel, nameof(playerModel));

        ent = GameObject.Find("Ent");
        Utility.LogErrorIfNull(ent, nameof(ent));

        animator = GetComponent<Animator>();
        Utility.LogErrorIfNull(animator, nameof(animator));
    }

    private void Update()
    {
        var lookAtThis = new Vector3 { x = playerModel.transform.position.x, y = transform.position.y, z = playerModel.transform.position.z };
        transform.LookAt(lookAtThis);
        ent.transform.LookAt(lookAtThis);

        // TODO: If time, consider adding animation
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
