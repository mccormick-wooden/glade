using UnityEngine;
using UnityEngine.Events;


public class AudioEventManager : MonoBehaviour
{
    public EventSound3D eventSound3DPrefab;

    [Header("Sword")]
    public AudioClip[] swordSwingAudio = null;
    private UnityAction<Vector3, int> swordSwingEventListener;
    public float[] swordSwingSoundDelays = null;
    public float[] swordSwingPitches = null;

    [Header("Crystal")]
    public AudioClip crystalCollisionAudio = null;
    private UnityAction<Vector3> crystalCollisionEventListener;


    void Awake()
    {
        // sword 
        swordSwingEventListener = new UnityAction<Vector3, int>(swordSwingEventHandler);
        crystalCollisionEventListener = new UnityAction<Vector3>(crystalCollisionEventHandler);
    }


    // Start is called before the first frame update
    void Start()
    {


    }

    void OnEnable()
    {
        EventManager.StartListening<SwordSwingEvent, Vector3, int>(swordSwingEventListener);
        EventManager.StartListening<CrystalCollisionEvent, Vector3>(crystalCollisionEventListener);
    }

    void OnDisable()
    {
        EventManager.StopListening<SwordSwingEvent, Vector3, int>(swordSwingEventListener);
        EventManager.StopListening<CrystalCollisionEvent, Vector3>(crystalCollisionEventListener);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void crystalCollisionEventHandler(Vector3 worldPos)
    {
        EventSound3D snd = Instantiate(eventSound3DPrefab, worldPos, Quaternion.identity, null);

        snd.audioSrc.minDistance = 1f;
        snd.audioSrc.maxDistance = 500f;
        snd.audioSrc.spatialBlend = 1f;
        snd.audioSrc.pitch = 1f;
        snd.audioSrc.volume = 0.5f;
        snd.audioSrc.PlayOneShot(crystalCollisionAudio);
    }

    void swordSwingEventHandler(Vector3 worldPos, int whichSwing)
    {
        EventSound3D snd = Instantiate(eventSound3DPrefab, worldPos, Quaternion.identity, null);

       /* if (whichSwing == 0)
        {
            snd.audioSrc.clip = this.swordSwingAudio[whichSwing];
            snd.audioSrc.PlayDelayed(swordSwingSoundDelays[whichSwing]);
        }
        else if (whichSwing == 1)
        {*/
            snd.audioSrc.clip = swordSwingAudio[whichSwing];
            snd.audioSrc.pitch = swordSwingPitches[whichSwing];
            snd.audioSrc.PlayDelayed(swordSwingSoundDelays[whichSwing]);
        //}
    }
}
