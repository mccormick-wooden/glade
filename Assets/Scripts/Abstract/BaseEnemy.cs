using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rigidBody;
    public GameObject playerGameObject;
    public Transform Player;

    // Height should be filled in by a specific subclass 
    public float Height; 

    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        Player = GameObject.Find("Player").transform;
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
