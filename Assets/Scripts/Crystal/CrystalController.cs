using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.LightningBolt;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SphereCollider))]
public class CrystalController : MonoBehaviour
{
    public int CrystalID => gameObject.GetInstanceID();

    [Header("Crystal Effect")]
    public float EffectMultiplier = 1;
    public bool effectActive { get; private set; }
    public AudioSource crystalAudio;
    public AudioSource growAudio;

    [Header("Lightning Effect")]
    public GameObject LightningPrefab;
    public Transform LightningSource;
    public AudioSource lightningAudio;

    [SerializeField]
    private float effectRadius = 5;

    private SphereCollider effectEnableCollider;

    private Animator anim;
    private int activatorsClose = 0;

    private Dictionary<string, GameObject> lightningByName = new Dictionary<string, GameObject>();

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
    private void FixedUpdate()
    {
        /* Reset activators close before OnTriggerStay
         * The reason I need to recalculate this every cycle is to properly
         * keep track of activators if effect scripts are disabled
         */
        activatorsClose = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (activatorsClose == 0)
            effectActive = false;

        anim.SetInteger("ActivatorsClose", activatorsClose);
    }

    void OnTriggerStay(Collider other)
    {
        BaseCrystalEffect crystalEffect = other.GetComponent<BaseCrystalEffect>();
        if (crystalEffect != null && crystalEffect.enabled)
        {
            activatorsClose++;
        }

        CrystalDamageEffect damageEffect = other.GetComponent<CrystalDamageEffect>();
        if (damageEffect != null)
        {
            /* If the damageEffect script has been disabled, stop zapping it */
            if (!damageEffect.enabled)
                DestroyLightning(other.gameObject);

            /* If the damageEffect is enabled and it isn't already being zapped, zap it */
            if (damageEffect.enabled && !lightningByName.ContainsKey(damageEffect.name))
                CreateLightning(damageEffect.gameObject);
        }
    }

    private void OnEnable()
    {
        // Let the activators retrigger the crystal
        activatorsClose = 0;
        anim.Play("Growing");
        growAudio.Play();
    }

    private void OnDisable()
    {
        DestroyAllLightning();
    }

    private void CreateLightning(GameObject target)
    {
        GameObject lightningObj = Instantiate(LightningPrefab, LightningSource);
        lightningObj.name = $"Lightning-{target.name}";
        LightningBoltScript lightning = lightningObj.GetComponent<LightningBoltScript>();
        lightning.StartObject = LightningSource.gameObject;
        lightning.EndObject = target.GetComponent<CrystalDamageEffect>()?.LightningTarget;
        lightningByName[target.gameObject.name] = lightningObj;
        lightningAudio.Play();
    }

    private void DestroyAllLightning()
    {
        foreach (string targetName in lightningByName.Keys)
        {
            GameObject target = GameObject.Find(targetName);
            DestroyLightning(target);
        }
    }

    private void DestroyLightning(GameObject target)
    {
        string targetName = target.gameObject.name;
        if (lightningByName.ContainsKey(targetName))
        {
            Destroy(lightningByName[targetName]);
            lightningByName.Remove(targetName);
            lightningAudio.Stop();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CrystalDamageEffect damageEffect = other.GetComponent<CrystalDamageEffect>();
        if (damageEffect != null)
            DestroyLightning(other.gameObject);
    }
}
