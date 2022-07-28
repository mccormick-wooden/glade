using UnityEngine;
using UnityEngine.Events;


public class AudioEventManager : MonoBehaviour
{
    public EventSound3D eventSound3DPrefab;

    // sword 
    private UnityAction<Vector3, int> swordSwingEventListener;
    private UnityAction<Vector3, int> swordHitEventListener;
    private UnityAction<Vector3> appleHitGrassEventListener;
    private UnityAction<Vector3> playerEatAppleEventListener;

    public AudioClip[] swordSwingAudio = null;
    public float[] swordSwingSoundDelays = null;
    public float[] swordSwingPitches = null;

    public AudioClip[] swordHitAudio = null;
    public float[] swordHitSoundDelays = null;
    public float[] swordHitPitches = null;

    public AudioClip appleHitGrass = null;

    public EventSound3D swordHitSound;

    public int appleHitGrassStartOffsetPCMs;

    public AudioClip playerEatApple = null;
    public int playerEatAppleOffsetPCMs;

    // walking 


    // running


    void Awake()
    {
        // sword 
        swordSwingEventListener = new UnityAction<Vector3, int>(swordSwingEventHandler);
        swordHitEventListener = new UnityAction<Vector3, int>(swordHitEventHandler);
        appleHitGrassEventListener = new UnityAction<Vector3>(appleHitGrassEventHandler);
        playerEatAppleEventListener = new UnityAction<Vector3>(playerEatAppleEventHandler);
    }


    // Start is called before the first frame update
    void Start()
    {


    }

    void OnEnable()
    {
        EventManager.StartListening<SwordSwingEvent, Vector3, int>(swordSwingEventListener);
        EventManager.StartListening<SwordHitEvent, Vector3, int>(swordHitEventListener);
        EventManager.StartListening<AppleHitGrassEvent, Vector3>(appleHitGrassEventListener);
        EventManager.StartListening<PlayerEatAppleEvent, Vector3>(playerEatAppleEventListener);
    }

    // Update is called once per frame
    void Update()
    {

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
        snd.audioSrc.Play();
    }
}
