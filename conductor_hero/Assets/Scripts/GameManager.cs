using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Instrument = AudioManager.Instrument;

/*
 * Playtesting notes
 * our song of choice seems a bit too hard for those with limited music experience (obviously)
 * In general though, if you are into it, it seems to work pretty well. 
 * 
 * Other comments on improvements:
 * Lines interpolating between the spheres to show your movement pattern.
 *      Small pulsating circles from where the spheres are on beat once they are invisible. 
 *      So you know where the beat is without having the spheres.
 *      A better visual indicator for what the first beat of a measure is. 
 *          Bigger explosion on the first beat for example. 
 * 
 * +1/+10 popups on the score whenever score is added. 
 * 
 * Maybe have the progress bar pulse as well (we'll see)
 * 
 * We're counting points for before the song starts, we want to fix that
 *      Count points on the first cue and onwards. 
 * 
 * Improve visuals on the ring around cues
 * 
 * When was it most boring?
 *      When there are the least cue's
 *      (The song is also probably too long, but thats not a relevant thing to change,
 *      but smaller play sessions would probably increase engagement when your interaction
 *      with the world is fairly static)
 * 
 * We need to calibrate the height of the spheres related to the headset height
 *      Different heights gives different arm lengths as well. 
 *      So we might want to scale the distance away from cue's depending on height as well
 *      NOTE: Seems to have improved when the player actually stands on the sweet spot which is where the 
 *      feet are supposed to be on the level. 
 * 
 * A better visual indicator for when the song starts would be nice
 *      Having the player start the song instead. 
 *      Song starts when player hits first cue for example.
 *      We need to give start/stop points for giving out score. 
 * 
 * Most people seem to be too focused on the bubbles
 *      They dont seem to notice the environment as a result
 *      Letting players start when they want could give them the time to look around. 
 * 
 * Having the table at the bottom of your peripheral vision would be nice
 *      This is another scaling question
 * 
 * The controllers are a bit to heavy
 *
 * PMS noted how he held the controller at the bottom tip, so he could use it as a batton,
 *      This made it feel like the spheres weren't so far away.
 */

public class GameManager
    : MonoBehaviour
{
    [SerializeField]
    private Text m_scoreText;

    void Start()
    {
        // Create cue signal objects
        // Setup which beats we need to push on
        // These times are based on a temporary hack of starting the song 1 bar later, for testing
        // Need to be readjusted.
        m_sectionsToCueOnBeat = new Dictionary<int, Instrument>
        {
            // Update to 8 rather than + 2, result of earlier error
            { 2 + 6, Instrument.glock },
            { 2 + 38, Instrument.harpsichord },
            { 2 + 54, Instrument.violins_extra },
            { 2 + 102, Instrument.violins_extra },
            { 2 + 166, Instrument.harpsichord },
            { 2 + 198, Instrument.violins_extra },
            { 2 + 294, Instrument.glock }, // This should be all instruments, figure out how to do that
            { 2 + 614, Instrument.harpsichord },
            { 2 + 742, Instrument.glock }, // This should be all instruments, figure out how to do that

            ////// For testing
            //{ 2 + 8 + 6, Instrument.glock },
            //{ 2 + 8 + 54, Instrument.violins_extra },
            //{ 2 + 8 + 102, Instrument.violins_extra },
            //{ 2 + 8 + 166, Instrument.harpsichord },
            //{ 2 + 8 + 198, Instrument.violins_extra },
            //{ 2 + 8 + 294, Instrument.glock }, // This should be all instruments, figure out how to do that
            //{ 2 + 8 + 614, Instrument.harpsichord },
            //{ 2 + 8 + 742, Instrument.glock }, // This should be all instruments, figure out how to do 

            //// For testing
            //{ 2 + 8 + 8 + 6, Instrument.glock },
            //{ 2 + 8 + 8 + 54, Instrument.violins_extra },
            //{ 2 + 8 + 8 + 102, Instrument.violins_extra },
            //{ 2 + 8 + 8 + 166, Instrument.harpsichord },
            //{ 2 + 8 + 8 + 198, Instrument.violins_extra },
            //{ 2 + 8 + 8 + 294, Instrument.glock }, // This should be all instruments, figure out how to do that
            //{ 2 + 8 + 8 + 614, Instrument.harpsichord },
            //{ 2 + 8 + 8 + 742, Instrument.glock }, // This should be all instruments, figure out how to do

        };


        //float song_length = m_audioManager.GetSonglength();
        //double total_score = Math.Round(Convert.ToDouble(song_length) * bmpSec);
  

        m_metronome = GameObject.Find("Metronome").GetComponent<Metronome>();
        m_metronome.onBeatTickedCallback += new Metronome.OnBeatTickCallback(OnBeat);

        m_audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        m_audioManager.MuteInstrument(Instrument.glock, true);
        m_audioManager.MuteInstrument(Instrument.harpsichord, true);
        m_audioManager.MuteInstrument(Instrument.violins_extra, true);
        m_audioManager.MuteInstrument(Instrument.oboe, true);


        float bmpSec = (float)m_metronome.bpm / 60.0f;
        float songLength = m_audioManager.GetSonglength();

        m_maxScore = (int)(songLength * bmpSec) + m_sectionsToCueOnBeat.Count * 10;
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

    public void AddScore(int points)
    {
        m_score += points;

        //double bmpSec = m_metronome.bpm / 60;
        //float song_length = m_audioManager.GetSonglength();
        //double total_score = Math.Round(Convert.ToDouble(song_length) * bmpSec);
        string.Format("{0}/{1}", m_score, m_maxScore);
        m_scoreText.text = string.Format("{0}/{1}", m_score, m_maxScore);
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
    int m_maxScore;
}
