using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * When you first allow someone to cue an instrument you have to make sure 
 * that there are multiple cues for the instrument throughout the song 
 */

public class Cue
    : MonoBehaviour
{
    public void Start()
    {
        m_text = gameObject.GetComponentInChildren<Text>();

        // Fun fact: GetComponentInChildren also searches the parent so this is a hack to fix that. 
        m_trailRendererEdge = GetComponentsInChildren<TrailRenderer>()[1];
        m_trailRendererCore = GetComponent<TrailRenderer>();
        successPosition = GameObject.Find(gameObject.name + "_pos").GetComponent<Transform>();

        if (gameObject.name == "oboe")
        {
            m_animations = new List<Animation>
            {
                GameObject.Find(gameObject.name + "_anim_0").GetComponent<Animation>(),
                GameObject.Find(gameObject.name + "_anim_1").GetComponent<Animation>(),
                GameObject.Find(gameObject.name + "_anim_2").GetComponent<Animation>(),
            };

            m_bounces = new List<BeatBounce>
            {
                GameObject.Find(gameObject.name + "_anim_0").GetComponent<BeatBounce>(),
                GameObject.Find(gameObject.name + "_anim_1").GetComponent<BeatBounce>(),
                GameObject.Find(gameObject.name + "_anim_2").GetComponent<BeatBounce>(),
            };
        }
        else if (gameObject.name != "drums")
        {
            m_animations = new List<Animation>
            {
                GameObject.Find(gameObject.name + "_anim").GetComponent<Animation>(),
            };

            m_bounces = new List<BeatBounce>
            {
                GameObject.Find(gameObject.name + "_anim").GetComponent<BeatBounce>(),
            };
            
        }

        StopAnimation();
    }

    public void ReInit(GameManager gm, AudioManager am, Metronome met, GameManager.CueInfo info)
    {
        m_gameManager = gm;
        m_audioManager = am;
        m_metronome = met;
        m_beatCountdown = info.countdown;
        m_info = info;

        m_volumeState = VolumeState.none;

        Renderer r = gameObject.GetComponent<Renderer>();
        r.material.color = new Color(0.5f, 0.5f, 1.0f);


        Debug.LogFormat("Countdown: {0}", m_beatCountdown);

    }

    public void StartAnimation()
    {
        if (gameObject.name != "drums")
        {
            foreach (var i in m_animations)
                i.Play();

            foreach (var i in m_bounces)
                i.StartBouncing();
        }
    }

    public void StopAnimation()
    {
        if (gameObject.name != "drums")
        {
            foreach (var i in m_animations)
                i.Stop();

            foreach (var i in m_bounces)
                i.StopBouncing();
            
        }
    }

    void Update()
    {
        // Goldylocks number. want to increase sound 0.15 for each millisecond (ish)
        float volumeChange = 0.15f * Time.deltaTime * 100.0f;
        if (m_volumeState == VolumeState.mute)
        {
            float currVolume = m_audioManager.GetInstrumentVolume(m_info.instrument);
            currVolume -= volumeChange;
            m_audioManager.SetInstrumentVolume(m_info.instrument, currVolume);
            if (currVolume <= 0.0f)
                m_volumeState = VolumeState.none;
        }

        if (m_volumeState == VolumeState.unmute)
        {
            float currVolume = m_audioManager.GetInstrumentVolume(m_info.instrument);
            currVolume += volumeChange;
            m_audioManager.SetInstrumentVolume(m_info.instrument, currVolume);
            if (currVolume >= 1.0f)
                m_volumeState = VolumeState.none;
        }

        if (m_state == State.idle)
            return;

        if (m_state == State.rising)
            PlayRisingAnimation();

        if (m_state == State.countdown)
            PlayCountdownAnimation();

        if (m_state == State.success)
            PlaySuccessAnimation();

        if (m_state == State.failed)
            PlayFailAnimation();
    }

    public void OnBeat(int beatID)
    {
        m_beatCountdown--;
        if (m_beatCountdown == m_info.beatToMute)
        {
            m_volumeState = VolumeState.mute;
            StopAnimation();
        }

        if (m_beatCountdown == m_info.startCueLogic)
            TransitionToState(State.rising);

        if (m_beatCountdown == 4)
            TransitionToState(State.countdown);

        // Hack for testing music
        //if (m_state == State.countdown && m_beatCountdown == 0)
        //    TransitionToState(State.success);

        // Comment back in when finished testing music
        if (m_state == State.countdown && m_beatCountdown <= -2)
        {
            TransitionToState(State.failed);
        }

        if (m_state == State.countdown)
        {
            m_text.text = m_beatCountdown.ToString();
        }
    }

    // Only relevant in editor
    public void OnMouseDown()
    {
        ReactToHit();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CueHand"))
            ReactToHit();
    }

    private void TransitionToState(State state)
    {
        // TODO: REFACTOR THIS SHIT!
        m_trailRendererEdge.enabled = false;
        m_state = state;
        if (state == State.countdown)
        {
            m_angle = 0.0f;
            m_trailRendererEdge.enabled = true;

            m_rotationSpeed = 360.0f / ((60.0f / (float)m_metronome.bpm) * 4.0f);
        }
        if (state == State.rising)
        {
            m_startTime = (float)AudioSettings.dspTime;
            m_length = Vector3.Distance(idlePosition.position, countdownPosition.position);

            // Multiplied by 3.0f instead of 4, as we are curently "in" a beat, 
            // and therefore only have 3 beats left to cover distance.
            m_speed = m_length / ((60.0f / (float)m_metronome.bpm) * 3.0f);
            gameObject.transform.position = idlePosition.position;
        }
        if (state == State.success)
        { 
            //m_audioManager.MuteInstrument(m_trackToUnmute, false);
            m_volumeState = VolumeState.unmute;
            m_startTime = (float)AudioSettings.dspTime;
            m_length = Vector3.Distance(countdownPosition.position, successPosition.position);
            m_speed = m_length * 4.0f;
            gameObject.transform.position = countdownPosition.position;

            m_trailRendererCore.enabled = true;

            StartAnimation();
        }
        if (state == State.failed)
        {
            Debug.Log("Fail");
            Renderer r = gameObject.GetComponent<Renderer>();
            r.material.color = new Color(1.0f, 0.0f, 0.0f);
            m_startTime = (float)AudioSettings.dspTime;
            m_length = Vector3.Distance(countdownPosition.position, idlePosition.position);
            m_speed = m_length * 2.0f;
            gameObject.transform.position = idlePosition.position;
        }
    }

    private void PlaySuccessAnimation()
    {
        float distCovered = ((float)AudioSettings.dspTime - m_startTime) * m_speed;
        float fracJourney = distCovered / m_length;
        transform.position = Vector3.Lerp(countdownPosition.position, successPosition.position, fracJourney);
    }

    private void PlayFailAnimation()
    {
        float distCovered = ((float)AudioSettings.dspTime - m_startTime) * m_speed;
        float fracJourney = distCovered / m_length;
        transform.position = Vector3.Lerp(countdownPosition.position, idlePosition.position, fracJourney);
    }

    private void PlayRisingAnimation()
    {
        float distCovered = ((float)AudioSettings.dspTime - m_startTime) * m_speed;
        float fracJourney = distCovered / m_length;
        transform.position = Vector3.Lerp(idlePosition.position, countdownPosition.position, fracJourney);
    }

    private void PlayCountdownAnimation()
    {
        if (m_angle <= 360.0f)
        {
            m_angle += Time.deltaTime * m_rotationSpeed;
        }

        // Loses initial y rotation since we're rotating around (0, 0, 1)
        transform.rotation = Quaternion.AngleAxis(m_angle, Vector3.forward);
    }

    private void ReactToHit()
    {
        // Deal with case where the cue is not ready to be hit. (for example being in rising mode, or in countdown)


        // Should use beatID in addition to countdown
        // But need to wait until that functionality is there
        if (m_beatCountdown >= -1 && m_beatCountdown <= 1)
        //if (m_beatCountdown == 0)
        {
            Debug.LogFormat("You hit!");
            Renderer r = gameObject.GetComponent<Renderer>();
            r.material.color = new Color(0.0f, 1.0f, 0.0f);

            TransitionToState(State.success);
            m_gameManager.AddScore(10);
        }
    }

    enum State
    {
        idle,
        rising, // Use 1 bar rising out from the table
        countdown, // 1 bar counting down from 4 to 1. 
        success, // Interpolate to the orchestra
        failed, // Fall limp to the ground
    };

    enum VolumeState
    {
        none,
        unmute,
        mute,
    };

    AudioManager m_audioManager;
    Metronome m_metronome;
    GameManager.CueInfo m_info;
    List<BeatBounce> m_bounces;

    VolumeState m_volumeState;

    int m_beatCountdown;
    State m_state;

    float m_angle;
    float m_rotationSpeed;

    float m_startTime;
    float m_speed;

    float m_length;

    Text m_text;

    TrailRenderer m_trailRendererEdge;

    [SerializeField]
    Transform countdownPosition;

    [SerializeField]
    Transform idlePosition;

    [SerializeField]
    Transform successPosition;

    [SerializeField]
    Transform failPosition;

    Transform m_prevPos;
    Transform m_nextPos;

    [SerializeField]
    GameManager m_gameManager;

    List<Animation> m_animations;

    private TrailRenderer m_trailRendererCore;
}
