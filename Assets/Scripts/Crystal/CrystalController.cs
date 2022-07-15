using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.LightningBolt;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SphereCollider))]
public class CrystalController : MonoBehaviour
{
    public int CrystalID => gameObject.GetInstanceID();

    public float EffectMultiplier = 1;
    public bool effectActive { get; private set; }

    public GameObject LightningPrefab;
    public EventSound3D eventSound3DPrefab;
    public AudioClip lightningSound;

    public Transform LightningSource;

    [SerializeField]
    private float effectRadius = 5;

    private SphereCollider effectEnableCollider;

    private Animator anim;
    private int activatorsClose = 0;

    private Dictionary<int, GameObject> lightningById = new Dictionary<int, GameObject>();

    private void Awake()
    {
        Utility.LogErrorIfNull(LightningPrefab, "LightningPrefab", "Requires lightning prefab for generating lightning effects.");
        if (LightningSource == null) LightningSource = transform;

        anim = GetComponent<Animator>();
        Utility.LogErrorIfNull(anim, "animator", "Crystal Controller requires an animator.");

        // Set the activator collider to the effect radius.
        effectEnableCollider = GetComponent<SphereCollider>();
        Utility.LogErrorIfNull(anim, "sphere collider", "Crystal Controller requires a sphere collider.");
        if (effectEnableCollider != null)
            effectEnableCollider.radius = effectRadius;
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetInteger("ActivatorsClose", activatorsClose);
    }

    private void OnEnable()
    {
        // Let the activators retrigger the crystal
        activatorsClose = 0;
        anim.Play("Growing");
    }

    private void CreateLightning(GameObject target)
    {
        GameObject lightningObj = Instantiate(LightningPrefab, LightningSource);
        lightningObj.name = $"Lightning{lightningObj.GetInstanceID()}";
        LightningBoltScript lightning = lightningObj.GetComponent<LightningBoltScript>();
        lightning.StartObject = LightningSource.gameObject;
        lightning.EndObject = target.GetComponent<CrystalDamageEffect>().LightningTarget;
        lightningById[target.gameObject.GetInstanceID()] = lightningObj;
        EventSound3D sound = Instantiate(eventSound3DPrefab, transform.position, Quaternion.identity, lightningObj.transform);
        sound.name = $"{name}-LightningSound-{target.name}";
        sound.audioSrc.clip = lightningSound;
        sound.audioSrc.loop = true;
        sound.audioSrc.Play();
    }

    private void DestroyLightning(GameObject target)
    {
        int instanceId = target.gameObject.GetInstanceID();
        if (lightningById.ContainsKey(instanceId))
        {
            Destroy(lightningById[instanceId]);
            lightningById.Remove(instanceId);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (null != other.GetComponent<BaseCrystalEffect>())
        {
            activatorsClose++;
            Debug.Log($"{name}: Activator arrived. {activatorsClose} in vicinity");
            if (null != other.GetComponent<CrystalDamageEffect>())
            {
                CreateLightning(other.gameObject);
            }
        }
        if (activatorsClose > 0) effectActive = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (null != other.GetComponent<BaseCrystalEffect>())
        {
            activatorsClose--;
            Debug.Log($"{name}: Activator left. {activatorsClose} in vicinity.");
            DestroyLightning(other.gameObject);
        }
        if (activatorsClose <= 0)
        {
            activatorsClose = 0;
            effectActive = false;
        }
    }
}
