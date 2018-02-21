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
        m_beatCountdown = countdown;
        m_trackToUnmute = trackToUnmute;
        m_audioManager = audioManager;

        m_mode = Mode.countdown;
    }

    void Update()
    {
        if (m_mode == Mode.idle)
            return;

        if (m_mode == Mode.success)
            PlaySuccessAnimation();

        if (m_mode == Mode.failed)
            PlayFailAnimation();

        // Sort of doing two modes here really, going out of the board, up to the user, and displaying the countdown.
        // if (m_mode == Mode.countdown)
        //     DisplayCountdown();


    }

    public void PlaySuccessAnimation()
    {

    }

    public void PlayFailAnimation()
    {

    }

    public void OnBeat(int beatID)
    {
        m_beatCountdown--;
        // Should we potentially also mute instrument here 1 beat before it starts, to ensure that the instrument isn't on when cued?
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

    private void ReactToHit()
    {
        // Should use beatID in addition to countdown
        // But need to wait until that functionality is there
        if (m_beatCountdown >= -1 && m_beatCountdown <= 1)
        {
            Debug.LogFormat("You hit!");
            Renderer r = gameObject.GetComponent<Renderer>();
            r.material.color = new Color(1.0f, 0.0f, 0.0f);

            m_audioManager.MuteInstrument(m_trackToUnmute, false);
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

    
}
