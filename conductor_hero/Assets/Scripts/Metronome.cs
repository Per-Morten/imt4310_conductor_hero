using System;
using UnityEngine;

// Based on: https://gist.github.com/bzgeb/c298c6189c73b2cf777c

public class Metronome : MonoBehaviour
{
    public double bpm = 175.0;

    double nextBeat = 0.0;
    double prevBeat = 0.0;

    bool ticked = true;

    double secondsPerBeat;

    AudioManager manager;
    
    public double OnBeat()
    {
        var now = AudioSettings.dspTime;
        var prevBeatDiff = now - (prevBeat + secondsPerBeat / 2.0);
        var nextBeatDiff = (nextBeat - secondsPerBeat / 2.0) - now;

        Debug.Log(string.Format("prev {0} next {1}", prevBeatDiff, nextBeatDiff));

        return Math.Min(prevBeatDiff, nextBeatDiff);
    }

    void Start()
    {
        secondsPerBeat = 60.0 / bpm;

        double startTick = AudioSettings.dspTime;

        nextBeat = startTick + secondsPerBeat;

        manager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        manager.PlayMusic(AudioManager.MusicTrack.conductor_hero_orchestral, nextBeat);
    }

    void LateUpdate()
    {
        if (!ticked && nextBeat >= AudioSettings.dspTime)
        {
            ticked = true;
            manager.PlaySoundEffect(AudioManager.SfxTrack.ButtonPress, AudioSettings.dspTime);
        }
    }

    void FixedUpdate()
    {
        double dspTime = AudioSettings.dspTime;

        while (dspTime >= nextBeat)
        {
            ticked = false;
            prevBeat = nextBeat;
            nextBeat += secondsPerBeat;
        }
    }

}