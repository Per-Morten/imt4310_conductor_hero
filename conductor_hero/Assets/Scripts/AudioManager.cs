using System;
using System.Collections.Generic;
using UnityEngine;

// Temporary for testing Metronome.
// This can't actually be used for production as it doesn't support multiple music tracks.
public class AudioManager
    : MonoBehaviour
{
	public enum MusicTrack
    {
		None,
		conductor_hero_orchestral,
		conductor_hero_orchestral_debug,
	};

	public enum SfxTrack
    {
		None,
		ButtonPress,
    }

    public void Awake()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
		//musicSource.mute = true;
		musicSource.volume = 0.15f;


		sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
		sfxSource.volume = 1.0f;

        musicTracks = new Dictionary<MusicTrack, AudioClip>();
        foreach (MusicTrack value in Enum.GetValues(typeof(MusicTrack)))
        {
            if (value != MusicTrack.None)
            {
                var val = Resources.Load<AudioClip>("Sounds/" + value.ToString());
                if (!val)
                {
                    Debug.LogWarningFormat("Could not load file {0}", value.ToString());
                }
                musicTracks[value] = val;
            }
        }

        sfxTracks = new Dictionary<SfxTrack, AudioClip>();
        foreach (SfxTrack value in Enum.GetValues(typeof(SfxTrack)))
        {
            sfxTracks[value] = Resources.Load<AudioClip>("Sounds/" + value.ToString());

        }

    }

	public void PlayMusic(MusicTrack track, double time)
    {
        if (track != currentMusicTrack)
        {
            currentMusicTrack = track;

            musicSource.Stop();
            musicSource.clip = musicTracks[track];
			musicSource.PlayScheduled(time);
            //musicSource.Play();
        }
    }

	public void PlaySoundEffect(SfxTrack track, double time)
    {
        //sfxSource.PlayOneShot(sfxTracks[track]);
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
    private MusicTrack currentMusicTrack = MusicTrack.None;

    private Dictionary<MusicTrack, AudioClip> musicTracks;
}
