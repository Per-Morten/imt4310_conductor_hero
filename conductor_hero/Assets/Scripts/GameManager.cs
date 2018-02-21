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
            { 5, Instrument.glock },
            { 36, Instrument.harpsichord },
            { 50, Instrument.violins_extra }
        };

        m_cueSignals = new List<Cue>
        {
            GameObject.Find("Sphere").GetComponent<Cue>(),
            GameObject.Find("Sphere (1)").GetComponent<Cue>(),
            GameObject.Find("Sphere (2)").GetComponent<Cue>(),
            GameObject.Find("Sphere (3)").GetComponent<Cue>(),

        };


        m_metronome = GameObject.Find("Metronome").GetComponent<Metronome>();
        m_metronome.onBeatTickedCallback += new Metronome.OnBeatTickCallback(OnBeat);

        m_audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        m_audioManager.MuteInstrument(Instrument.glock, true);
        m_audioManager.MuteInstrument(Instrument.harpsichord, true);
        m_audioManager.MuteInstrument(Instrument.violins_extra, true);

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnBeat(int beatID)
    {
        Debug.LogFormat("BeatID{0}", beatID);
        if (m_sectionsToCueOnBeat.ContainsKey(beatID))
        {
            //Debug.LogFormat("In if statement");
            int index = (int)m_sectionsToCueOnBeat[beatID];

            Renderer r = m_cueSignals[index].GetComponent<Renderer>();
            r.material.color = new Color(0.5f, 0.5f, 1.0f);

            // - 1 because we are on the beat where this will happen
            m_cueSignals[index].ReInit(m_cueCountdown - 1, m_sectionsToCueOnBeat[beatID], m_audioManager);
        }
    }

    // Responsibilities
    // - Create the cues when needed 
    // - Check if the cues are hit
    //      - Check if they are hit on beat
    //      - Mute and unmute tracks
    // - 

    [SerializeField]
    List<Cue> m_cueSignals;

    // Holds when they should start to be queued in
    // In this case, 2 bars (i.e. 8 beats) before they are activated

    [SerializeField]
    Dictionary<int, Instrument> m_sectionsToCueOnBeat;

    [SerializeField]
    AudioManager m_audioManager;

    [SerializeField] // How many beats before the actual beat do we notify the user?
    int m_cueCountdown = 4;

    Metronome m_metronome;
}
