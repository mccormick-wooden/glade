using UnityEngine;

public class BeaconOrbiter : MonoBehaviour
{
    public GameObject parentBeacon;

    void Update()
    {
        transform.RotateAround(parentBeacon.transform.position, Vector3.up, 50 * Time.deltaTime);
    }
}
