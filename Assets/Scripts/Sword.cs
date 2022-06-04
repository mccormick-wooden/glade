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
        if (other.tag == "Enemy" && IsSwinging)
        {
            Debug.Log("Collision!");
            other.gameObject.SetActive(false);
        }
    }
}
