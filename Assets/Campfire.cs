using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventManager.TriggerEvent<CampfireStartEvent, Vector3>(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
