using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalManager : MonoBehaviour
{
    GameObject[] Crystals;

    // Start is called before the first frame update
    void Awake()
    {
        Crystals = GameObject.FindGameObjectsWithTag("Crystal");
    }

    // Update is called once per frame
    void Update()
    {
    }
}
