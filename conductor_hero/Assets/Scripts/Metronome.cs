using UnityEngine;

// Based on: https://gist.github.com/bzgeb/c298c6189c73b2cf777c

public class Metronome : MonoBehaviour
{
	public double bpm = 175.0;

	double nextTick = 0.0;
	bool ticked = true;

	AudioManager manager;


	void Start()
	{
		double startTick = AudioSettings.dspTime;

		nextTick = startTick + (60.0 / bpm);

		manager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
		manager.PlayMusic(AudioManager.MusicTrack.conductor_hero_orchestral, nextTick);
	}

	void LateUpdate()
	{
		if (!ticked && nextTick >= AudioSettings.dspTime)
		{
			ticked = true;
			manager.PlaySoundEffect(AudioManager.SfxTrack.ButtonPress, AudioSettings.dspTime);
		}
	}

	// Just an example OnTick here
	void OnTick()
	{
		Debug.Log("Tick");
		// GetComponent<AudioSource>().Play();
	}

	void FixedUpdate()
	{
		double timePerTick = 60.0f / bpm;
		double dspTime = AudioSettings.dspTime;

		while (dspTime >= nextTick)
		{
			ticked = false;
			nextTick += timePerTick;
		}
	}
}