using System;
using System.Collections.Generic;
using UnityEngine;

// Temporary for testing Metronome.
// This can't actually be used for production as it doesn't support multiple music tracks.
public class AudioManager
    : MonoBehaviour
{
    public enum InstrumentTrack
    {
        None,
        conductor_hero_orchestral_layer_bass,
        conductor_hero_orchestral_layer_drums,
        conductor_hero_orchestral_layer_glock,
        conductor_hero_orchestral_layer_harpsichord,
        conductor_hero_orchestral_layer_oboe,
        conductor_hero_orchestral_layer_violas_lead,
        conductor_hero_orchestral_layer_violins_extra,
        conductor_hero_orchestral_layer_violins_lead
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
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.mute = true;
        musicSource.volume = 0.15f;


        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.volume = 1.0f;

        m_instruments = new List<AudioSource>();
        foreach (InstrumentTrack value in Enum.GetValues(typeof(InstrumentTrack)))
        {
            if (value != InstrumentTrack.None)
            {
                var source = gameObject.AddComponent<AudioSource>();
                m_instruments.Add(source);

                var val = Resources.Load<AudioClip>("Sounds/" + value.ToString());
                if (!val)
                {
                    Debug.LogWarningFormat("Couldnt find {0} file", value.ToString());
                }
                source.clip = val;
            }
        }

        
        sfxTracks = new Dictionary<SfxTrack, AudioClip>();
        foreach (SfxTrack value in Enum.GetValues(typeof(SfxTrack)))
        {
            sfxTracks[value] = Resources.Load<AudioClip>("Sounds/" + value.ToString());

        }

    }

    public void PlayMusic(double time)
    {
        foreach (var val in m_instruments)
        {
            val.PlayScheduled(time);
        }
    }

    public void PlaySoundEffect(SfxTrack track, double time)
    {
        sfxSource.clip = sfxTracks[track];
        sfxSource.PlayScheduled(time);
    }

    public bool MuteMusic
    {
        set
        {
            musicSource.mute = value;
        }
        get
        {
            return musicSource.mute;
        }
    }

    public bool MuteSfx
    {
        set
        {
            sfxSource.mute = value;
        }
        get
        {
            return sfxSource.mute;
        }
    }

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private Dictionary<SfxTrack, AudioClip> sfxTracks;

    private List<AudioSource> m_instruments;
}
