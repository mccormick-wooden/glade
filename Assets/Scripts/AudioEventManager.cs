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

    public AudioClip[] swordSwingAudio = null;
    public float[] swordSwingSoundDelays = null;
    public float[] swordSwingPitches = null;

    public AudioClip[] swordHitAudio = null;
    public float[] swordHitSoundDelays = null;
    public float[] swordHitPitches = null;

    public EventSound3D swordHitSound;

    [Header("Apple")]
    public AudioClip appleHitGrass = null;

    public int appleHitGrassStartOffsetPCMs;

    public AudioClip playerEatApple = null;
    public int playerEatAppleOffsetPCMs;

    [Header("Crystal")]
    public AudioClip crystalCollisionAudio = null;
    private UnityAction<Vector3> crystalCollisionEventListener;
    
    public AudioClip[] playerFootstep = null;
    public int[] playerFootstepOffsetPCMs;

    public AudioClip[] playerInjured = null;
    public int[] playerInjuredOffsetPCMs;

    // walking 


    // running


    void Awake()
    {
        // sword 
        swordSwingEventListener = new UnityAction<Vector3, int>(swordSwingEventHandler);
        crystalCollisionEventListener = new UnityAction<Vector3>(crystalCollisionEventHandler);
        swordHitEventListener = new UnityAction<Vector3, int>(swordHitEventHandler);
        appleHitGrassEventListener = new UnityAction<Vector3>(appleHitGrassEventHandler);
        playerEatAppleEventListener = new UnityAction<Vector3>(playerEatAppleEventHandler);
        playerFootstepEventListener = new UnityAction<Vector3, int>(playerFootstepEventHandler);
        playerHurtEventListener = new UnityAction<Vector3>(playerHurtEventHandler);
    }


    // Start is called before the first frame update
    void Start()
    {


    }

    void OnEnable()
    {
        EventManager.StartListening<SwordSwingEvent, Vector3, int>(swordSwingEventListener);
        EventManager.StartListening<CrystalCollisionEvent, Vector3>(crystalCollisionEventListener);
        EventManager.StartListening<SwordHitEvent, Vector3, int>(swordHitEventListener);
        EventManager.StartListening<AppleHitGrassEvent, Vector3>(appleHitGrassEventListener);
        EventManager.StartListening<PlayerEatAppleEvent, Vector3>(playerEatAppleEventListener);
        EventManager.StartListening<PlayerFootstepEvent, Vector3, int>(playerFootstepEventListener);
        EventManager.StartListening<PlayerHurtEvent, Vector3>(playerHurtEventListener);
    }

<<<<<<< HEAD
    void OnDisable()
    {
        EventManager.StopListening<SwordSwingEvent, Vector3, int>(swordSwingEventListener);
        EventManager.StopListening<CrystalCollisionEvent, Vector3>(crystalCollisionEventListener);
        EventManager.StopListening<SwordHitEvent, Vector3, int>(swordHitEventListener);
        EventManager.StopListening<AppleHitGrassEvent, Vector3>(appleHitGrassEventListener);
        EventManager.StopListening<PlayerEatAppleEvent, Vector3>(playerEatAppleEventListener);
    }
=======
>>>>>>> 16ecbdc (Added footsteps)

    // Update is called once per frame
    void Update()
    {

    }

    void crystalCollisionEventHandler(Vector3 worldPos)
    {
        EventSound3D snd = Instantiate(eventSound3DPrefab, worldPos, Quaternion.identity, null);

        snd.audioSrc.minDistance = 1f;
        snd.audioSrc.maxDistance = 500f;
        snd.audioSrc.spatialize = true;
        snd.audioSrc.spatialBlend = 1f;
        snd.audioSrc.pitch = 1f;
        snd.audioSrc.volume = 0.5f;
        snd.audioSrc.PlayOneShot(crystalCollisionAudio);
    }

    void swordSwingEventHandler(Vector3 worldPos, int whichSwing)
    {
        EventSound3D snd = Instantiate(eventSound3DPrefab, worldPos, Quaternion.identity, null);
        snd.audioSrc.clip = swordSwingAudio[whichSwing];
        snd.audioSrc.pitch = swordSwingPitches[whichSwing];
        snd.audioSrc.spatialize = true;
        snd.audioSrc.spatialBlend = 1;
        snd.audioSrc.PlayDelayed(swordSwingSoundDelays[whichSwing]);
    }



    void swordHitEventHandler(Vector3 worldPos, int whichSwing)
    {
        if (swordHitSound && swordHitSound.audioSrc.isPlaying)
            return;

        swordHitSound = Instantiate(eventSound3DPrefab, worldPos, Quaternion.identity, null);

        swordHitSound.audioSrc.spatialize = true;
        swordHitSound.audioSrc.spatialBlend = 1;
        swordHitSound.audioSrc.clip = swordHitAudio[whichSwing];
        swordHitSound.audioSrc.volume = .5f;
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
        snd.audioSrc.Play();
    }

    void playerEatAppleEventHandler(Vector3 worldPos)
    {
        EventSound3D snd = Instantiate(eventSound3DPrefab, worldPos, Quaternion.identity, null);
        snd.audioSrc.spatialize = true;
        snd.audioSrc.spatialBlend = 1;
        snd.audioSrc.clip = playerEatApple;
        snd.audioSrc.timeSamples = playerEatAppleOffsetPCMs;
        //snd.audioSrc.pitch = 0.8f;
        snd.audioSrc.volume = 0.6f;
        snd.audioSrc.Play();
    }

    void playerFootstepEventHandler(Vector3 worldPos, int whilchStep)
    {
        EventSound3D snd = Instantiate(eventSound3DPrefab, worldPos, Quaternion.identity, null);
        snd.audioSrc.spatialize = true;
        snd.audioSrc.spatialBlend = 1;
        snd.audioSrc.clip = playerFootstep[whilchStep];
        snd.audioSrc.timeSamples = playerFootstepOffsetPCMs[whilchStep];
        //snd.audioSrc.pitch = 0.8f;
        snd.audioSrc.volume = 0.6f;
        snd.audioSrc.Play();
    }

    void playerHurtEventHandler(Vector3 worldPos)
    {
        int whichOof = Random.Range(0, playerInjured.Length - 1);

        EventSound3D snd = Instantiate(eventSound3DPrefab, worldPos, Quaternion.identity, null);
        snd.audioSrc.spatialize = true;
        snd.audioSrc.spatialBlend = 1;
        snd.audioSrc.clip = playerInjured[whichOof];
        snd.audioSrc.timeSamples = playerInjuredOffsetPCMs[whichOof];
        //snd.audioSrc.pitch = 0.8f;
        snd.audioSrc.volume = 0.6f;
        snd.audioSrc.Play();
    }

}
