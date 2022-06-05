using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public bool IsSwinging { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        IsSwinging = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsSwinging && other.tag == "Enemy")
        {
            //other.gameObject.SetActive(false);
            other.gameObject.GetComponent<Rigidbody>().AddForce(25f, 25f, 25f, ForceMode.Impulse);
        }
    } 

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Enemy") // I guess we'll need a collection of tags to interact with
        {
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }
}
