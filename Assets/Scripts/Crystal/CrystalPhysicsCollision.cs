using UnityEngine;

public class CrystalPhysicsCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision c)
    {
        if (c.impulse.magnitude > 0.5f)
        {
            EventManager.TriggerEvent<CrystalCollisionEvent, Vector3>(c.GetContact(0).point);
        }
    }
}
