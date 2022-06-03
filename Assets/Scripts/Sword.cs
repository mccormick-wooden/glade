using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public bool isSwinging { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        isSwinging = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && isSwinging)
        {
            Debug.Log("Collision!");
            other.gameObject.SetActive(false);
        }
    }
}
