using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Instrument = AudioManager.Instrument;

public class GameManager
    : MonoBehaviour
{ 
    void Start()
    {
        // Create cue signal objects
        // Setup which beats we need to push on
        // These times are based on a temporary hack of starting the song 1 bar later, for testing
        // Need to be readjusted.
        m_sectionsToCueOnBeat = new Dictionary<int, Instrument>
        {
            { 6, Instrument.glock },
            { 38, Instrument.harpsichord },
            { 54, Instrument.violins_extra }
        };

        m_metronome = GameObject.Find("Metronome").GetComponent<Metronome>();
        m_metronome.onBeatTickedCallback += new Metronome.OnBeatTickCallback(OnBeat);

        m_audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        m_audioManager.MuteInstrument(Instrument.glock, true);
        m_audioManager.MuteInstrument(Instrument.harpsichord, true);
        m_audioManager.MuteInstrument(Instrument.violins_extra, true);

    }

    void OnBeat(int beatID)
    {
        if (m_sectionsToCueOnBeat.ContainsKey(beatID))
        {
            int index = (int)m_sectionsToCueOnBeat[beatID];
            m_cueSignals[index].ReInit(m_cueCountdown, m_sectionsToCueOnBeat[beatID], m_audioManager, m_metronome);
        }
        foreach (var cue in m_cueSignals)
        {
            cue.OnBeat(beatID);
        }
    }

    public void AddScore(int points, int total_points)
    {
        m_score += points;
        m_total_user_moves += total_points;


        double bmpSec = m_metronome.bpm/60;

        float song_length =m_audioManager.GetSonglength();

        double total_score = Math.Round(Convert.ToDouble(song_length) / bmpSec);

        Debug.Log(string.Format("Points: {0}, Total: {1}, totalScore: {2}", m_score, m_total_user_moves, total_score));
    }

    [SerializeField]
    List<Cue> m_cueSignals;

    // Holds when they should start to be queued in
    // In this case, 2 bars (i.e. 8 beats) before they are activated

    [SerializeField]
    Dictionary<int, Instrument> m_sectionsToCueOnBeat;

    [SerializeField]
    AudioManager m_audioManager;

    [SerializeField] // How many beats before the actual beat do we notify the user?
    int m_cueCountdown = 8;

    Metronome m_metronome;

    int m_score;
    int m_total_user_moves;
    int total_score;
}
