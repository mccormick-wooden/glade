using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rigidBody;
    public Transform Player;

    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
    }

    private void UpdateAnimations()
    {
    }

    private void ApplyTransforms()
    {
        transform.LookAt(Player);

        transform.position += transform.forward * 5 * Time.deltaTime;
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        ApplyTransforms();
    }
}
