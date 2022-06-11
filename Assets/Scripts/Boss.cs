using Assets.Scripts.Damageable;
using UnityEngine;

// Blatantly copied from Enemy.cs - did not want to step over changes in Enemy.cs for other scenes
public class Boss : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rigidBody;
    private AnimateDamageable damageManager;
    public Transform Player;

    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        damageManager = GetComponent<AnimateDamageable>();
    }

    protected virtual void UpdateAnimations() { }

    private void ApplyTransforms()
    {
        // TODO: Ideally transforms and triggered animations are handled separately
        var currentPositionIgnoringY = new Vector3(
            transform.position.x,
            Player.transform.position.y,
            transform.position.z
        );
        var testRotation = Quaternion.LookRotation(
            Player.transform.position - currentPositionIgnoringY
        );

        float angle = Quaternion.Angle(transform.rotation, testRotation);
        // Want to minimize how much rotating is happening when small changes are needed
        if (angle >= 10.0f)
        {
            var relativeRotation = Quaternion.LookRotation(
                Player.transform.position - transform.position
            );
            rigidBody.MoveRotation(
                Quaternion.Slerp(transform.rotation, relativeRotation, 1 * Time.deltaTime)
            );
            animator.SetBool("isTurning", true);
        }
        else
        {
            animator.SetBool("isTurning", false);
        }

        float distanceToPlayer = Vector3.Distance(Player.transform.position, transform.position);
        if (distanceToPlayer >= 16.0f)
        {
            rigidBody.MovePosition(transform.position + (transform.forward * (5 * Time.deltaTime)));
            animator.SetBool("isTurning", false);
            animator.SetBool("Walk Forward", true);
        }
        else
        {
            animator.SetBool("Walk Forward", false);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (damageManager.HasHp)
            UpdateAnimations();
    }

    private void FixedUpdate()
    {
        if (damageManager.HasHp)
            ApplyTransforms();
    }
}
