using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRootMotion : MonoBehaviour
{
    public float animationSpeed;
    public float rootMovementSpeed;
    public float rootTurnSpeed;

    private Animator animator;
    private Rigidbody rigidBody;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("Animator could not be found");

        rigidBody = GetComponent<Rigidbody>();
        if (rigidBody == null)
            Debug.LogError("Rigid body could not be found");
    }

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        animator.speed = animationSpeed;
    }

    private void OnAnimatorMove()
    {
        Vector3 newRootPosition;
        Quaternion newRootRotation;

        newRootPosition = animator.rootPosition;
        newRootRotation = animator.rootRotation;

        newRootPosition = Vector3.LerpUnclamped(
            transform.position,
            newRootPosition,
            rootMovementSpeed
        );

        rigidBody.MovePosition(newRootPosition);
        rigidBody.MoveRotation(newRootRotation);
    }
}
