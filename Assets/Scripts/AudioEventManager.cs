using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class AudioEventManager : MonoBehaviour
{
    public EventSound3D eventSound3DPrefab;

    // background 
    public AudioClip[] backgroundMusic = null;
    private UnityAction<int> playMusicEventListener;
    private int whichSong;

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

        // background
        whichSong = 0;
        playMusicEventListener = new UnityAction<int>(playMusicEventHandler);

        //playMusicEventListener = new UnityAction<Vector3, int>(playMusicEventHandler);
    }


    // Start is called before the first frame update
    void Start()
    {


    }

    void OnEnable()
    {
        EventManager.StartListening<SwordSwingEvent, Vector3, int>(swordSwingEventListener);
        EventManager.StartListening<PlayMusicEvent, int>(playMusicEventListener);
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
            snd.audioSrc.clip = this.swordSwingAudio[whichSwing];
            snd.audioSrc.pitch = swordSwingPitches[whichSwing];
            snd.audioSrc.PlayDelayed(swordSwingSoundDelays[whichSwing]);
        //}
    }

    public void playMusicEventHandler(int whichSong)
    {
        // need some way to loop music here?
        // switch songs

        // does this create a sound that we lose hearing of over space?
        EventSound3D snd = Instantiate(eventSound3DPrefab, new Vector3(0,0,0), Quaternion.identity, null);

        snd.audioSrc.clip = this.backgroundMusic[0];
        snd.audioSrc.Play();
    }
}
