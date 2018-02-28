using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Feedback from test:
// - Maybe going a bit to fast
// - 

public class Cue
    : MonoBehaviour
{
    public void Start()
    {
        GameObject.Find("Metronome").GetComponent<Metronome>().onBeatTickedCallback += new Metronome.OnBeatTickCallback(OnBeat);
        m_text = gameObject.GetComponentInChildren<Text>();
    }

    public void ReInit(int countdown, AudioManager.Instrument trackToUnmute, AudioManager audioManager)
    {
        // Log which beat id it is here.
        m_beatCountdown = countdown;
        m_trackToUnmute = trackToUnmute;
        m_audioManager = audioManager;

        Renderer r = gameObject.GetComponent<Renderer>();
        r.material.color = new Color(0.5f, 0.5f, 1.0f);

        TransitionToState(State.rising);

        Debug.LogFormat("Got {0} beats", m_beatCountdown);
        m_text.text = "";
    }

    void Update()
    {
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

        // Sort of doing two modes here really, going out of the board, up to the user, and displaying the countdown.
        // if (m_mode == Mode.countdown)
        //     DisplayCountdown();


    }

    public void OnBeat(int beatID)
    {
        m_beatCountdown--;
        // Switch state
        if (m_beatCountdown == 4)
        {
            TransitionToState(State.countdown);   
        }
        if (m_state == State.countdown && m_beatCountdown <= -2 && !m_isHit)
        {
            TransitionToState(State.failed);
        }
        if (m_beatCountdown == 2)
        {
            m_audioManager.MuteInstrument(m_trackToUnmute, true);
        }

        // Should we potentially also mute instrument here 1 beat before it starts, to ensure that the instrument isn't on when cued?
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
        ReactToHit();
    }

    private void TransitionToState(State state)
    {
        // TODO: REFACTOR THIS SHIT!
        m_state = state;
        if (state == State.rising)
        {
            m_startTime = (float)AudioSettings.dspTime;
            m_length = Vector3.Distance(idlePosition.position, countdownPosition.position);

            // 4 beats before it starts
            m_speed = m_length / ((m_startTime + (60.0f / 165.0f) * 3.0f) - m_startTime);
            gameObject.transform.position = idlePosition.position;
        }
        if (state == State.success)
        {
            m_startTime = (float)AudioSettings.dspTime;
            m_length = Vector3.Distance(countdownPosition.position, successPosition.position);
            m_speed = m_length * 4.0f;
            gameObject.transform.position = countdownPosition.position;
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

            m_audioManager.MuteInstrument(m_trackToUnmute, false);
            m_isHit = true;
            TransitionToState(State.success);
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

    AudioManager m_audioManager;
    AudioManager.Instrument m_trackToUnmute;
    int m_beatCountdown;
    bool m_isHit;
    State m_state;

    float m_startTime;
    float m_speed;

    float m_length;

    Text m_text;

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
}
