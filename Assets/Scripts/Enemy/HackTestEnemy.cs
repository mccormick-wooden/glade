using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackTestEnemy : BaseEnemy
{
    protected override void ApplyTransforms()
    {
        transform.LookAt(GameObject.Find("PlayerModel")?.gameObject?.transform);
        transform.position += transform.forward * 2 * Time.deltaTime;
    }
}
