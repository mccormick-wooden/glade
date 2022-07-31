using UnityEngine;
using UnityEngine.Events;


public class AudioEventManager : MonoBehaviour
{
    public EventSound3D eventSound3DPrefab;

    [Header("Sword")]
    private UnityAction<Vector3, int> swordSwingEventListener;
    private UnityAction<Vector3, int> swordHitEventListener;
    private UnityAction<Vector3> appleHitGrassEventListener;
    private UnityAction<Vector3> playerEatAppleEventListener;
    private UnityAction<Vector3, int> playerFootstepEventListener;
    private UnityAction<Vector3> playerHurtEventListener;
    private UnityAction<Vector3> fairyAOEAttackEventListener;
    private UnityAction<Vector3> campfireEventListener;

    private UnityAction<Vector3, AudioClip, float> monsterTakeDamageEventListener;
    private UnityAction<Vector3, AudioClip, float, float> monsterDieEventListener;

    public AudioClip[] swordSwingAudio = null;
    public float[] swordSwingSoundDelays = null;
    public float[] swordSwingPitches = null;
    [Range(0f,1f)]
    public float swordSwingVolume = 1f;

    public AudioClip[] swordHitAudio = null;
    public float[] swordHitSoundDelays = null;
    public float[] swordHitPitches = null;
    [Range(0f,1f)]
    public float swordHitVolume = 0.5f;

    public EventSound3D swordHitSound;

    [Header("Apple")]
    public AudioClip appleHitGrass = null;
    [Range(0f,1f)]
    public float appleHitGrassVolume = 1f;

    public int appleHitGrassStartOffsetPCMs;

    public AudioClip playerEatApple = null;
    [Range(0f,1f)]
    public float playerEatAppleVolume = 0.6f;
    public int playerEatAppleOffsetPCMs;

    [Header("Crystal")]
    public AudioClip crystalCollisionAudio = null;
    [Range(0f,1f)]
    public float crystalCollisionVolume = 0.5f;
    private UnityAction<Vector3> crystalCollisionEventListener;

    public AudioClip crystalDeathAudio = null;
    [Range(0f,1f)]
    public float crystalDeathVolume = 1f;
    private UnityAction<Vector3> crystalDeathEventListener;
    
    [Header("Player")]
    public AudioClip[] playerFootstep = null;
    [Range(0f,1f)]
    public float playerFootstepVolume = 0.6f;
    public int[] playerFootstepOffsetPCMs;

    public EventSound3D playerInjuredSound;
    [Range(0f,1f)]
    public float playerHurtVolume = 0.6f;
    public AudioClip[] playerInjured = null;
    public int[] playerInjuredOffsetPCMs;

    [Header("Environment")]
    public AudioClip campfire = null;
    [Range(0f,1f)]
    public float campfireVolume = 0.5f;

    [Header("Enemies")]
    public AudioClip fairyAOEAttack = null;
    public int fairyAOEAttackOffsetPCMs;
    [Range(0f,1f)]
    public float fairyAOEVolume = 1f;

    void Awake()
    {
        swordSwingEventListener = new UnityAction<Vector3, int>(swordSwingEventHandler);
        crystalDeathEventListener = new UnityAction<Vector3>(crystalDeathEventHandler);
        crystalCollisionEventListener = new UnityAction<Vector3>(crystalCollisionEventHandler);
        swordHitEventListener = new UnityAction<Vector3, int>(swordHitEventHandler);
        appleHitGrassEventListener = new UnityAction<Vector3>(appleHitGrassEventHandler);
        playerEatAppleEventListener = new UnityAction<Vector3>(playerEatAppleEventHandler);
        playerFootstepEventListener = new UnityAction<Vector3, int>(playerFootstepEventHandler);
        playerHurtEventListener = new UnityAction<Vector3>(playerHurtEventHandler);
        fairyAOEAttackEventListener = new UnityAction<Vector3>(fairyAOEAttackEventHandler);
        monsterTakeDamageEventListener = new UnityAction<Vector3, AudioClip, float>(monsterTakeDamageEventHandler);
        monsterDieEventListener = new UnityAction<Vector3, AudioClip, float, float>(monsterDieEventHandler);
        campfireEventListener = new UnityAction<Vector3>(CampFireSoundEventHandler);
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    void OnEnable()
    {
        EventManager.StartListening<SwordSwingEvent, Vector3, int>(swordSwingEventListener);
        EventManager.StartListening<CrystalDeathEvent, Vector3>(crystalDeathEventListener);
        EventManager.StartListening<CrystalCollisionEvent, Vector3>(crystalCollisionEventListener);
        EventManager.StartListening<SwordHitEvent, Vector3, int>(swordHitEventListener);
        EventManager.StartListening<AppleHitGrassEvent, Vector3>(appleHitGrassEventListener);
        EventManager.StartListening<PlayerEatAppleEvent, Vector3>(playerEatAppleEventListener);
        EventManager.StartListening<PlayerFootstepEvent, Vector3, int>(playerFootstepEventListener);
        EventManager.StartListening<PlayerHurtEvent, Vector3>(playerHurtEventListener);
        EventManager.StartListening<FairyAOEAttackEvent, Vector3>(fairyAOEAttackEventHandler);
        EventManager.StartListening<MonsterTakeDamageEvent, Vector3, AudioClip, float>(monsterTakeDamageEventHandler);
        EventManager.StartListening<MonsterDieEvent, Vector3, AudioClip, float, float>(monsterDieEventHandler);
        EventManager.StartListening<CampfireStartEvent, Vector3>(CampFireSoundEventHandler);

    }


    void OnDisable()
    {
        EventManager.StopListening<SwordSwingEvent, Vector3, int>(swordSwingEventListener);
        EventManager.StopListening<CrystalDeathEvent, Vector3>(crystalDeathEventListener);
        EventManager.StopListening<CrystalCollisionEvent, Vector3>(crystalCollisionEventListener);
        EventManager.StopListening<SwordHitEvent, Vector3, int>(swordHitEventListener);
        EventManager.StopListening<AppleHitGrassEvent, Vector3>(appleHitGrassEventListener);
        EventManager.StopListening<PlayerEatAppleEvent, Vector3>(playerEatAppleEventListener);
        EventManager.StopListening<PlayerFootstepEvent, Vector3, int>(playerFootstepEventListener);
        EventManager.StopListening<PlayerHurtEvent, Vector3>(playerHurtEventListener);
        EventManager.StopListening<FairyAOEAttackEvent, Vector3>(fairyAOEAttackEventHandler);
        EventManager.StopListening<MonsterTakeDamageEvent, Vector3, AudioClip, float>(monsterTakeDamageEventHandler);
        EventManager.StopListening<MonsterDieEvent, Vector3, AudioClip, float, float>(monsterDieEventHandler);
        EventManager.StopListening<CampfireStartEvent, Vector3>(CampFireSoundEventHandler);

    }


    // Update is called once per frame
    void Update()
    {

    }

    void crystalDeathEventHandler(Vector3 worldPos)
    {
        EventSound3D snd = Instantiate(eventSound3DPrefab, worldPos, Quaternion.identity, null);

        snd.audioSrc.minDistance = 1f;
        snd.audioSrc.maxDistance = 500f;
        snd.audioSrc.spatialize = true;
        snd.audioSrc.spatialBlend = 1f;
        snd.audioSrc.pitch = 1f;
        snd.audioSrc.volume = crystalDeathVolume;
        snd.audioSrc.PlayOneShot(crystalDeathAudio);
    }

    void crystalCollisionEventHandler(Vector3 worldPos)
    {
        EventSound3D snd = Instantiate(eventSound3DPrefab, worldPos, Quaternion.identity, null);

        snd.audioSrc.minDistance = 1f;
        snd.audioSrc.maxDistance = 500f;
        snd.audioSrc.spatialize = true;
        snd.audioSrc.spatialBlend = 1f;
        snd.audioSrc.pitch = 1f;
        snd.audioSrc.volume = crystalCollisionVolume;
        snd.audioSrc.PlayOneShot(crystalCollisionAudio);
    }

    void swordSwingEventHandler(Vector3 worldPos, int whichSwing)
    {
        EventSound3D snd = Instantiate(eventSound3DPrefab, worldPos, Quaternion.identity, null);
        snd.audioSrc.clip = swordSwingAudio[whichSwing];
        snd.audioSrc.pitch = swordSwingPitches[whichSwing];
        snd.audioSrc.volume = swordSwingVolume;
        snd.audioSrc.PlayDelayed(swordSwingSoundDelays[whichSwing]);
    }



    void swordHitEventHandler(Vector3 worldPos, int whichSwing)
    {
        if (swordHitSound && swordHitSound.audioSrc.isPlaying)
            return;

        swordHitSound = Instantiate(eventSound3DPrefab, worldPos, Quaternion.identity, null);

        swordHitSound.audioSrc.clip = swordHitAudio[whichSwing];
        swordHitSound.audioSrc.volume = swordHitVolume;
        swordHitSound.audioSrc.pitch = swordHitPitches[whichSwing];
        swordHitSound.audioSrc.PlayDelayed(swordHitSoundDelays[whichSwing]);
    }


    void appleHitGrassEventHandler(Vector3 worldPos)
    {
        EventSound3D snd = Instantiate(eventSound3DPrefab, worldPos, Quaternion.identity, null);
        snd.audioSrc.spatialize = true;
        snd.audioSrc.spatialBlend = 1;
        snd.audioSrc.clip = appleHitGrass;
        snd.audioSrc.timeSamples = appleHitGrassStartOffsetPCMs;
        snd.audioSrc.volume = appleHitGrassVolume;
        snd.audioSrc.Play();
    }

    void playerEatAppleEventHandler(Vector3 worldPos)
    {
        EventSound3D snd = Instantiate(eventSound3DPrefab, worldPos, Quaternion.identity, null);
        snd.audioSrc.clip = playerEatApple;
        snd.audioSrc.timeSamples = playerEatAppleOffsetPCMs;
        snd.audioSrc.volume = playerEatAppleVolume;
        snd.audioSrc.Play();
    }

    void playerFootstepEventHandler(Vector3 worldPos, int whilchStep)
    {
        EventSound3D snd = Instantiate(eventSound3DPrefab, worldPos, Quaternion.identity, null);
        snd.audioSrc.clip = playerFootstep[whilchStep];
        snd.audioSrc.timeSamples = playerFootstepOffsetPCMs[whilchStep];
        snd.audioSrc.volume = playerFootstepVolume;
        snd.audioSrc.Play();
    }

    void playerHurtEventHandler(Vector3 worldPos)
    {
        if (playerInjuredSound && playerInjuredSound.audioSrc.isPlaying)
            return;


        int whichOof = Random.Range(0, playerInjured.Length - 1);

        playerInjuredSound = Instantiate(eventSound3DPrefab, worldPos, Quaternion.identity, null);
        playerInjuredSound.audioSrc.clip = playerInjured[whichOof];
        playerInjuredSound.audioSrc.timeSamples = playerInjuredOffsetPCMs[whichOof];
        playerInjuredSound.audioSrc.volume = playerHurtVolume;
        playerInjuredSound.audioSrc.Play();
    }

    void fairyAOEAttackEventHandler(Vector3 worldPos)
    {
        EventSound3D snd = Instantiate(eventSound3DPrefab, worldPos, Quaternion.identity, null);
        snd.audioSrc.spatialize = true;
        snd.audioSrc.spatialBlend = 1;
        snd.audioSrc.clip = fairyAOEAttack;
        snd.audioSrc.timeSamples = fairyAOEAttackOffsetPCMs;
        snd.audioSrc.volume = fairyAOEVolume;
        snd.audioSrc.Play();
    }

    void monsterTakeDamageEventHandler(Vector3 worldPos, AudioClip clip, float pitch)
    {
        EventSound3D snd = Instantiate(eventSound3DPrefab, worldPos, Quaternion.identity, null);
        snd.audioSrc.spatialize = true;
        snd.audioSrc.spatialBlend = 1;
        snd.audioSrc.clip = clip;
        snd.audioSrc.Play();
    }

    void monsterDieEventHandler(Vector3 worldPos, AudioClip clip, float pitch, float volume)
    {
        EventSound3D snd = Instantiate(eventSound3DPrefab, worldPos, Quaternion.identity, null);
        snd.audioSrc.spatialize = true;
        snd.audioSrc.volume = volume;
        snd.audioSrc.spatialBlend = 1;
        snd.audioSrc.pitch = pitch;
        snd.audioSrc.clip = clip;
        snd.audioSrc.Play();
    }

    void CampFireSoundEventHandler(Vector3 worldPos)
    {
        EventSound3D fireSnd = Instantiate(eventSound3DPrefab, worldPos, Quaternion.identity, null);
        fireSnd.audioSrc.spatialize = true;
        fireSnd.audioSrc.spatialBlend = 1;
        fireSnd.audioSrc.clip = campfire;
        fireSnd.audioSrc.volume = campfireVolume;
        fireSnd.audioSrc.loop = true;
        fireSnd.audioSrc.Play();
    }
}
