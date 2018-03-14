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
        m_text = gameObject.GetComponentInChildren<Text>();
        m_trailRenderer = gameObject.GetComponentInChildren<TrailRenderer>();
    }

    public void ReInit(int countdown, AudioManager.Instrument trackToUnmute, AudioManager audioManager, Metronome metronome)
    {
        m_beatCountdown = countdown;
        m_trackToUnmute = trackToUnmute;
        m_audioManager = audioManager;
        m_metronome = metronome;

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
    }

    public void OnBeat(int beatID)
    {
        m_beatCountdown--;
        if (m_beatCountdown == 4)
        {
            TransitionToState(State.countdown);
        }

        // Debug fail state
        if (m_state == State.countdown && m_beatCountdown <= -2)
        {
            TransitionToState(State.failed);
        }

        // Muting instrument to ensure that the track is not playing from before
        if (m_beatCountdown == 2)
        {
            m_audioManager.MuteInstrument(m_trackToUnmute, true);
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
        m_trailRenderer.enabled = false;
        m_state = state;
        if (state == State.countdown)
        {
            m_angle = 0.0f;
            m_trailRenderer.enabled = true;

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

            m_audioManager.MuteInstrument(m_trackToUnmute, false);
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

    AudioManager m_audioManager;
    AudioManager.Instrument m_trackToUnmute;
    Metronome m_metronome;

    int m_beatCountdown;
    State m_state;

    float m_angle;
    float m_rotationSpeed;

    float m_startTime;
    float m_speed;

    float m_length;

    Text m_text;

    TrailRenderer m_trailRenderer;

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
}
