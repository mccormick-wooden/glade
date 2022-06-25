using Beacons;
using UnityEngine;

public class CrashedBeacon : MonoBehaviour
{
    /* I don't think we need this class because IDamageable provides all the behavior we currently care about on the beacon, 
     * but if we want to implement other behavior on the beacon, that could go here.
     * 
     * Presence of this script could also be a good way to coerce the type on death since otherwise 
     * we only know that the dying thing is a generic GameObject - although we do have alternate options for coercing a type, e.g. tags
     * 
     * Leaving for now  */
}
