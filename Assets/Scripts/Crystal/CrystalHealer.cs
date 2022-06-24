using Assets.Scripts.Abstract;
using UnityEngine;

[RequireComponent(typeof(BaseDamageable))]
[RequireComponent(typeof(Collider))]
public class CrystalHealer : MonoBehaviour
{
    [Tooltip("Nominal healing rate. May be multiplied by stronger crystals.")]
    [SerializeField]
    private float hpPerSecond;

    private BaseDamageable health;
    private Collider myCollider;

    void Awake()
    {
        // Need to be damageable in order to heal
        if (null == (health = GetComponent<BaseDamageable>()))
            Debug.LogError("Couldn't find damageable component.");

        // Need to have a collider in order to know when within effect radius
        if (null == (myCollider = GetComponent<Collider>()))
            Debug.LogError("Couldn't find collider component.");
    }


    private void Start()
    {
        RegisterWithCrystals();
    }

    private void RegisterWithCrystals()
    {
        // Register with all crystals (whether inactive or active) as a
        // collider that can activate the crystal
        CrystalController[] crystals = GameObject.FindObjectsOfType<CrystalController>(true);
        foreach (CrystalController crystal in crystals)
        {
            crystal.RegisterActivator(myCollider);
        }
    }

    void OnEnable()
    {
        EventManager.StartListening<CrystalEffectEvent, Vector3, float, float>(CrystalEffectEventHandler);
    }

    private void OnDisable()
    {
        EventManager.StopListening<CrystalEffectEvent, Vector3, float, float>(CrystalEffectEventHandler);
    }

    private void CrystalEffectEventHandler(Vector3 position, float effectRadius, float effectMultiplier)
    {
        // If we're within the effetRadius of the crystal event, apply healing with multiplier
        var distance = (position - transform.position).magnitude;
        if (distance <= effectRadius)
            health.Heal(hpPerSecond * effectMultiplier);
    }
}
