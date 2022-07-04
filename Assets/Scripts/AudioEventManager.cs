using UnityEngine;
using UnityEngine.Events;


public class AudioEventManager : MonoBehaviour
{
    public EventSound3D eventSound3DPrefab;

    // sword 
    public AudioClip[] swordSwingAudio = null;
    private UnityAction<Vector3, int> swordSwingEventListener;
    public float[] swordSwingSoundDelays = null;
    public float[] swordSwingPitches = null;

    // walking 


    // running


    void Awake()
    {
        // sword 
        swordSwingEventListener = new UnityAction<Vector3, int>(swordSwingEventHandler);
    }


    // Start is called before the first frame update
    void Start()
    {


    }

    void OnEnable()
    {
        EventManager.StartListening<SwordSwingEvent, Vector3, int>(swordSwingEventListener);
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
}
