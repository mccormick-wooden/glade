using System;
using UnityEngine;
using Abstract;

namespace Beacons
{
    
    
    public class BeaconSpawner : BaseSpawner
    {
        public GameObject fireball;

        public override void Spawn()
        {
            //var down = Vector3.down;
            Instantiate(fireball, transform.position, transform.rotation);
            //base.Spawn();
        }
    }
}
