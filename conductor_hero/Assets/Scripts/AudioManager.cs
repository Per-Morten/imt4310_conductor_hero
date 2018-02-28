using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager
    : MonoBehaviour
{
    public enum Instrument
    {
        harpsichord,
        violins_extra,
        glock,
        oboe, // These needs to come first! As they are also used elsewhere
        bass,
        drums,
        violas_lead,
        violins_lead
    };

    public enum SfxTrack
    {
        None,
        ButtonPress,
        conductor_hero_orchestral,
        conductor_hero_orchestral_debug,
    }

    public void Awake()
    {
        m_sfxSource = gameObject.AddComponent<AudioSource>();
        m_sfxSource.loop = false;
        m_sfxSource.volume = 1.0f;

        m_instruments = new List<AudioSource>();
        foreach (Instrument value in Enum.GetValues(typeof(Instrument)))
        {
            var source = gameObject.AddComponent<AudioSource>();
            m_instruments.Add(source);

            var val = Resources.Load<AudioClip>("Sounds/conductor_hero_orchestral_layer_" + value.ToString());
            if (!val)
            {
                Debug.LogWarningFormat("Couldnt find {0} file", value.ToString());
            }
            source.clip = val;
            //source.mute = true;
        }

        m_sfxTracks = new Dictionary<SfxTrack, AudioClip>();
        foreach (SfxTrack value in Enum.GetValues(typeof(SfxTrack)))
        {
            m_sfxTracks[value] = Resources.Load<AudioClip>("Sounds/" + value.ToString());
        }
    }

    public void PlayMusic(double time)
    {
        foreach (var val in m_instruments)
        {
            val.PlayScheduled(time);
        }
    }

    public void StopMusic()
    {
        foreach (var val in m_instruments)
        {
            val.Stop();
        }
    }

    public void MuteInstrument(Instrument instrument, bool isMuted)
    {
        m_instruments[(int)instrument].mute = isMuted;
    }

    public void SetInstrumentVolume(Instrument instrument, float volume)
    {
        m_instruments[(int)instrument].volume = volume;
    }

    public float GetInstrumentVolume(Instrument instrument)
    {
        return m_instruments[(int)instrument].volume;
    }

    public void PlaySoundEffect(SfxTrack track, double time)
    {
        m_sfxSource.clip = m_sfxTracks[track];
        m_sfxSource.PlayScheduled(time);
    }

    public bool MuteSfx
    {
        set
        {
            m_sfxSource.mute = value;
        }
        get
        {
            return m_sfxSource.mute;
        }
    }

    AudioSource m_sfxSource;
    Dictionary<SfxTrack, AudioClip> m_sfxTracks;
    List<AudioSource> m_instruments;
}
