using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.LightningBolt;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Abstract;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SphereCollider))]
public class CrystalController : MonoBehaviour
{
    public int CrystalID => gameObject.GetInstanceID();
    public bool debugOutput = false;

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

    [Tooltip("Time (seconds) after death to remove crystal objects.")]
    [SerializeField]
    private float disableDelay = 30f;

    private SphereCollider effectEnableCollider;

    private Animator anim;
    private int activatorsClose = 0;

    private Dictionary<string, GameObject> lightningByName = new Dictionary<string, GameObject>();

    private Rigidbody[] crystalRBs;
    private bool selfDestruct = false;
    private float selfDestructTime = Mathf.Infinity;

    private void Awake()
    {
        crystalRBs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody body in crystalRBs)
            body.isKinematic = true;

        transform.Find("Crystalsv05").GetComponent<IDamageable>().Died += OnDied;

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

    private void ExplodeCrystals()
    {
        foreach (Rigidbody body in crystalRBs)
        {
            // this is ugly but it's crunch time.
            if (body.name != "Crystalsv05")
            {
                body.constraints = RigidbodyConstraints.None;
                body.useGravity = true;
                body.isKinematic = false;
                Vector3 forceDir = (body.transform.position - body.transform.parent.transform.position).normalized;
                float force = Random.Range(5, 10);
                float torque = Random.Range(10, 20);
                body.AddForce(forceDir * force, ForceMode.Impulse);
                body.AddTorque((new Vector3(Random.Range(0, 1), Random.Range(0, 1), Random.Range(0, 1))).normalized * torque, ForceMode.Impulse);
            }
        }
    }

    private void PerformCrystalDeathCleanup()
    {
        // Don't want the capsule collider since it is used for damage detection from player
        transform.Find("Crystalsv05").GetComponent<CapsuleCollider>().enabled = false;

        // Don't need the health bar or light anymore
        transform.Find("Crystalsv05/Hovering Health Bar")?.gameObject.SetActive(false);
        transform.Find("Crystalsv05/Light")?.gameObject.SetActive(false);
        
        // Don't won't want the crystal to spawn anymore if it's dead
        transform.Find("CrystalSpawner").GetComponent<CrystalSpawner>().spawnActive = false;

        // Disable the animator
        anim.enabled = false;

        // Crystal effect collider should be disabled
        GetComponent<SphereCollider>().radius = 0;
        GetComponent<SphereCollider>().enabled = false;

        // Need to destroy all existing lightning, disable audio, and disable effect flag
        DestroyAllLightning();
        effectActive = false;
        crystalAudio.Stop();
    }

    private void SelfDestruct(float destructDelay)
    {
        if (debugOutput)
            Debug.Log($"Self destruct in {destructDelay}");
        selfDestruct = true;
        selfDestructTime = Time.time + destructDelay;
    }

    private void OnDied(IDamageable dmg, string name, int id)
    {
        // Disable crystal without removing it, since they now are rigid bodies
        PerformCrystalDeathCleanup();

        // Set crystal pieces to RB and shoot them off
        ExplodeCrystals();

        // Issue event for death
        EventManager.TriggerEvent<CrystalDeathEvent, Vector3>(transform.position);

        // Self destruct in disableDelay seconds
        SelfDestruct(disableDelay);
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

        if (selfDestruct && Time.time > selfDestructTime)
        {
            if (debugOutput)
                Debug.Log($"Self destructing");
            Destroy(this.gameObject);
        }
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

    public bool IsZappingMe(GameObject target)
    {
        return lightningByName.ContainsKey(target.gameObject.name);
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
            Destroy(lightningByName[target.name]);
        }
        lightningByName.Clear();
        lightningAudio.Stop();
    }

    private void DestroyLightning(GameObject target)
    {
        string targetName = target.gameObject.name;
        if (lightningByName.ContainsKey(targetName))
        {
            Destroy(lightningByName[targetName]);
            lightningByName.Remove(targetName);
        }

        if (lightningByName.Count == 0)
            lightningAudio.Stop();
    }

    private void OnTriggerExit(Collider other)
    {
        CrystalDamageEffect damageEffect = other.GetComponent<CrystalDamageEffect>();
        if (damageEffect != null)
            DestroyLightning(other.gameObject);
    }
}
