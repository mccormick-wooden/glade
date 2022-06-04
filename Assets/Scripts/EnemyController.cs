using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rigidBody;
    public Transform Player;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
    }

    void UpdateAnimations()
    {
    }

    void ApplyTransforms()
    {
        transform.LookAt(Player);

        transform.position += transform.forward * 5 * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        ApplyTransforms();
        UpdateAnimations();
    }
}
