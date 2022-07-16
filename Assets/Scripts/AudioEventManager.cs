using UnityEngine;
using UnityEngine.Events;


public class AudioEventManager : MonoBehaviour
{
    public EventSound3D eventSound3DPrefab;

    // sword 
    private UnityAction<Vector3, int> swordSwingEventListener;
    private UnityAction<Vector3, int> swordHitEventListener;

    public AudioClip[] swordSwingAudio = null;
    public float[] swordSwingSoundDelays = null;
    public float[] swordSwingPitches = null;

    public AudioClip[] swordHitAudio = null;
    public float[] swordHitSoundDelays = null;
    public float[] swordHitPitches = null;

    EventSound3D swordHitSound;


    // walking 


    // running


    void Awake()
    {
        // sword 
        swordSwingEventListener = new UnityAction<Vector3, int>(swordSwingEventHandler);
        swordHitEventListener = new UnityAction<Vector3, int>(swordHitEventHandler);
    }


    // Start is called before the first frame update
    void Start()
    {


    }

    void OnEnable()
    {
        EventManager.StartListening<SwordSwingEvent, Vector3, int>(swordSwingEventListener);
        EventManager.StartListening<SwordHitEvent, Vector3, int>(swordHitEventListener);
    }

    // Update is called once per frame
    void Update()
    {

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

    void swordHitEventHandler(Vector3 worldPos, int whichSwing)
    {
        if (swordHitSound && swordHitSound.audioSrc.isPlaying)
            return;

        swordHitSound = Instantiate(eventSound3DPrefab, worldPos, Quaternion.identity, null);
        
        swordHitSound.audioSrc.clip = swordHitAudio[whichSwing];
        swordHitSound.audioSrc.volume = .5f;
        swordHitSound.audioSrc.pitch = swordHitPitches[whichSwing];
        swordHitSound.audioSrc.PlayDelayed(swordHitSoundDelays[whichSwing]);
    }
}
