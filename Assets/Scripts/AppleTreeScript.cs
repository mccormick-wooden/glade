using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleTreeScript : MonoBehaviour
{
    //Transform apple;
    [SerializeField]
    Rigidbody apple;

    [SerializeField]
    ParticleSystem treeHitParticles;

    // Start is called before the first frame update
    void Start()
    {
        apple = transform.parent.Find("apple").GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Weapon" || !other.gameObject.GetComponent<Sword>() && !other.gameObject.GetComponent<Sword>().InUse)
            return;

        ParticleSystem hit = Instantiate(treeHitParticles);
        hit.transform.position = other.transform.position;
        hit.Play();

        if (apple.isKinematic == false)
            return;

        Debug.Log("Apple fall!");
        apple.isKinematic = false;
    }
}
