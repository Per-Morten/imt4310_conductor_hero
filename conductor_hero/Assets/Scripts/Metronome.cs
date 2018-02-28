using System;
using UnityEngine;

// Based on: https://gist.github.com/bzgeb/c298c6189c73b2cf777c

public class Metronome
    : MonoBehaviour
{
    public delegate void OnBeatTickCallback(int updatedBeatID);
    public OnBeatTickCallback onBeatTickedCallback;

    public double bpm = 175.0;

    double m_nextBeat = 0.0;
    double m_prevBeat = 0.0;

    int m_beatID = 0;

    bool m_ticked = true;

    double m_secondsPerBeat = 0;

    AudioManager manager;

    public int beatID
    {
        get { return m_beatID; }
    }

    public double OnBeat()
    {
        var userClick = AudioSettings.dspTime;
        var nextToUser = userClick - Math.Abs(m_nextBeat);
        var prevToUser = userClick - Math.Abs(m_prevBeat);

        if (Math.Abs(nextToUser) < Math.Abs(prevToUser))
        {
            return nextToUser;
        }
        else
        {
            return prevToUser;
        }
    }

    void Start()
    {
        m_secondsPerBeat = 60.0 / bpm;

        double startTick = AudioSettings.dspTime;

        m_nextBeat = startTick + m_secondsPerBeat;

        manager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        //manager.PlayMusic(m_nextBeat);
    }

    void LateUpdate()
    {
        if (!m_ticked && m_nextBeat >= AudioSettings.dspTime)
        {
            m_beatID++;
            m_ticked = true;
            manager.PlaySoundEffect(AudioManager.SfxTrack.ButtonPress, AudioSettings.dspTime);

            if (onBeatTickedCallback != null)
            {
                onBeatTickedCallback(m_beatID);
            }
        }
    }

    void FixedUpdate()
    {
        double dspTime = AudioSettings.dspTime;

        while (dspTime >= m_nextBeat)
        {
            m_ticked = false;
            m_prevBeat = m_nextBeat;
            m_nextBeat += m_secondsPerBeat;

            if (m_beatID == 7)
            {
                manager.PlayMusic(m_nextBeat);
            }
        }
    }
}
