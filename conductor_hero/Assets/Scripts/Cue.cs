using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cue
    : MonoBehaviour
{
    public void Start()
    {
        GameObject.Find("Metronome").GetComponent<Metronome>().onBeatTickedCallback += new Metronome.OnBeatTickCallback(OnBeat);
    }

    public void ReInit(int countdown, AudioManager.Instrument trackToUnmute, AudioManager audioManager)
    {
        // Log which beat id it is here.
        m_beatCountdown = countdown;
        m_trackToUnmute = trackToUnmute;
        m_audioManager = audioManager;


        Renderer r = gameObject.GetComponent<Renderer>();
        r.material.color = new Color(0.5f, 0.5f, 1.0f);

        m_mode = Mode.rising;

        m_startTime = (float)AudioSettings.dspTime;
        m_length = Vector3.Distance(idlePosition.position, countdownPosition.position);

        // 4 beats before it starts
        m_speed = m_length / ((m_startTime + (60.0f / 175.0f) * 3.0f) - m_startTime);
        gameObject.transform.position = idlePosition.position;

        Debug.LogFormat("Got {0} beats", m_beatCountdown);
    }

    void Update()
    {
        if (m_mode == Mode.idle)
            return;

        if (m_mode == Mode.rising)
            PlayRisingAnimation();

        if (m_mode == Mode.countdown)
            PlayCountdownAnimation();

        if (m_mode == Mode.success)
            PlaySuccessAnimation();

        if (m_mode == Mode.failed)
            PlayFailAnimation();

        // Sort of doing two modes here really, going out of the board, up to the user, and displaying the countdown.
        // if (m_mode == Mode.countdown)
        //     DisplayCountdown();


    }


    public void OnBeat(int beatID)
    {
        m_beatCountdown--;
        // Should we potentially also mute instrument here 1 beat before it starts, to ensure that the instrument isn't on when cued?
        if (m_beatCountdown >= 0)
        {
            Debug.Log(m_beatCountdown);
        }

        if (m_mode == Mode.rising && m_beatCountdown <= -2 && !m_isHit)
        {
            Debug.Log("Fail");
            Renderer r = gameObject.GetComponent<Renderer>();
            r.material.color = new Color(1.0f, 0.0f, 0.0f);
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

    private void PlaySuccessAnimation()
    {

    }

    private void PlayFailAnimation()
    {

    }

    private void PlayRisingAnimation()
    {
        float distCovered = ((float)AudioSettings.dspTime - m_startTime) * m_speed;
        float fracJourney = distCovered / m_length;
        transform.position = Vector3.Lerp(idlePosition.position, countdownPosition.position, fracJourney);

            //m_mode = Mode.countdown;
    }

    private void PlayCountdownAnimation()
    {
        Debug.Log("Countdown");
    }

    private void ReactToHit()
    {
        Debug.Log("Click");
        // Should use beatID in addition to countdown
        // But need to wait until that functionality is there
        if (m_beatCountdown >= -1 && m_beatCountdown <= 1)
        {
            Debug.LogFormat("You hit!");
            Renderer r = gameObject.GetComponent<Renderer>();
            r.material.color = new Color(0.0f, 1.0f, 0.0f);

            m_audioManager.MuteInstrument(m_trackToUnmute, false);
            m_isHit = true;
            m_mode = Mode.success;
        }
    }

    enum Mode
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
    Mode m_mode;

    float m_startTime;
    float m_speed = (60.0f / 175.0f) * 8.0f;

    float m_length;

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
