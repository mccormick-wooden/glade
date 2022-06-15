using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaconFall : MonoBehaviour
{
    public AudioClip soundEffect;

    public void Start()
    {
        AudioSource.PlayClipAtPoint(soundEffect, transform.position);
    }

}
