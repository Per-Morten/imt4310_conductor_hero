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
 * Lines interpolating between the spheres to show your movement pattern. (DONE)
 *      Small pulsating circles from where the spheres are on beat once they are invisible.
 *      So you know where the beat is without having the spheres.
 *      A better visual indicator for what the first beat of a measure is.
 *          Bigger explosion on the first beat for example.
 *
 * +1/+10 popups on the score whenever score is added. (DONE)
 *
 * Maybe have the progress bar pulse as well (we'll see)
 *
 * We're counting points for before the song starts, we want to fix that
 *      Count points on the first cue and onwards.
 *
 * Improve visuals on the ring around cues (DONE)
 *
 * When was it most boring?
 *      When there are the least cue's
 *      (The song is also probably too long, but thats not a relevant thing to change,
 *      but smaller play sessions would probably increase engagement when your interaction
 *      with the world is fairly static)
 *
 * We need to calibrate the height of the spheres related to the headset height (DONE)
 *      Different heights gives different arm lengths as well.
 *      So we might want to scale the distance away from cue's depending on height as well
 *      NOTE: Seems to have improved when the player actually stands on the sweet spot which is where the 
 *      feet are supposed to be on the level. 
 * 
 * A better visual indicator for when the song starts would be nice (DONEISH)
 *      Having the player start the song instead. 
 *      Song starts when player hits first cue for example.
 *      We need to give start/stop points for giving out score.
 *
 * Most people seem to be too focused on the bubbles
 *      They dont seem to notice the environment as a result
 *      Letting players start when they want could give them the time to look around. (DONEISH)
 * 
 * Having the table at the bottom of your peripheral vision would be nice
 *      This is another scaling question
 *
 * The controllers are a bit to heavy
 *
 * PMS noted how he held the controller at the bottom tip, so he could use it as a batton,
 *      This made it feel like the spheres weren't so far away.
 *
 * PMS and Andreas know that the spheres are actually capsule colliders, so we know we don't have to directly
 *      hit the sphere.
 *
 * Field of view etc, you would probably be able to see more if you had greater field of view which you probably have
 *      in a newer/better headset.
 */

public class GameManager
    : MonoBehaviour
{
    void Start()
    {
        m_cueInfos = new List<CueInfo>
        {
            new CueInfo(trackStart: 4, logicStart: 2, muteStart: 4, instrument: Instrument.glock, trackLength: 16),
            new CueInfo(trackStart: 12, logicStart: 2, muteStart: 32, instrument: Instrument.harpsichord, trackLength: 8),
            new CueInfo(trackStart: 16, logicStart: 2, muteStart: 8, instrument: Instrument.violins_extra, trackLength: 5),
            new CueInfo(trackStart: 28, logicStart: 2, muteStart: 8, instrument: Instrument.violins_extra, trackLength: 16),
            new CueInfo(trackStart: 44, logicStart: 2, muteStart: 8, instrument: Instrument.harpsichord, trackLength: 16),
            new CueInfo(trackStart: 52, logicStart: 2, muteStart: 8, instrument: Instrument.violins_extra, trackLength: 8),
            new CueInfo(trackStart: 76, logicStart: 2, muteStart: 8, instrument: Instrument.glock, trackLength: 16),
            new CueInfo(trackStart: 84, logicStart: 2, muteStart: 32, instrument: Instrument.harpsichord, trackLength: 16),
            new CueInfo(trackStart: 84, logicStart: 2, muteStart: 32, instrument: Instrument.oboe, trackLength: 8),


            new CueInfo(trackStart: 156, logicStart: 2, muteStart: 8, instrument: Instrument.harpsichord, trackLength: 48),
            new CueInfo(trackStart: 188, logicStart: 2, muteStart: 8, instrument: Instrument.glock, trackLength: 16),
            new CueInfo(trackStart: 196, logicStart: 2, muteStart: 32, instrument: Instrument.oboe, trackLength: 8),


            //// Extra added tracks
            new CueInfo(trackStart: 12, logicStart: 2, muteStart: 46, instrument: Instrument.oboe, trackLength: 8),
        };

        m_cueInfos.Sort((lhs, rhs) =>
        {
            if (lhs.initBeat == rhs.initBeat)
                return 0;
            if (lhs.initBeat < rhs.initBeat)
                return -1;
            return 1;
        });

        m_metronome = GameObject.Find("Metronome").GetComponent<Metronome>();
        m_metronome.onBeatTickedCallback += new Metronome.OnBeatTickCallback(OnBeat);

        m_audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        m_audioManager.SetInstrumentVolume(Instrument.harpsichord, 0.0f);
        m_audioManager.SetInstrumentVolume(Instrument.glock, 0.0f);
        m_audioManager.SetInstrumentVolume(Instrument.violins_extra, 0.0f);
        m_audioManager.SetInstrumentVolume(Instrument.oboe, 0.0f);

        // For youtube recording due to licensing
        // m_audioManager.MuteInstrument(Instrument.drums, true);

        float bmpSec = (float)m_metronome.bpm / 60.0f;
        float songLength = m_audioManager.GetSonglength();

        m_maxScore = (int)(songLength * bmpSec) + m_cueInfos.Count * 10;
    }

    void OnBeat(int beatID)
    {
        while (m_cueInfos.Count > 0 && m_cueInfos[0].initBeat == beatID)
        {
            CueInfo info = m_cueInfos[0];
            m_cueInfos.RemoveAt(0);
            m_cueSignals[(int)info.instrument].ReInit(this, m_audioManager, m_metronome, info);
        }


        foreach (var cue in m_cueSignals)
        {
            cue.OnBeat(beatID);
        }

        // Hack for getting instruments to start playing on correct beat.
        if (beatID == 16)
        {
            m_cueSignals[(int)Instrument.violas_lead].StartAnimation();
            m_cueSignals[(int)Instrument.violins_lead].StartAnimation();
            m_cueSignals[(int)Instrument.bass].StartAnimation();
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

        var scoreFeedback = Instantiate(m_scoreUpdateFeedbackPrefab, m_scoreText.transform.parent);
        scoreFeedback.GetComponent<Text>().text = "+" + points.ToString();

    }

    [SerializeField]
    List<Cue> m_cueSignals;

    // Holds when they should start to be queued in
    // In this case, 2 bars (i.e. 8 beats) before they are activated

    public struct CueInfo
    {
        public CueInfo(int trackStart, int logicStart, int trackLength, Instrument instrument, int muteStart = 8)
        {
            initBeat = (trackStart * 4 - Math.Max(logicStart * 4, muteStart));
            countdown = trackStart * 4 - initBeat;
            startCueLogic = logicStart * 4 - 1;
            beatToMute = muteStart - 1;
            this.instrument = instrument;
            this.trackLength = trackLength * 4;
        }

        // At what beat does the instrument actually enter?
        // How many beats before do we initiate the logic?
        // How many beats before do we mute the track?

        public int trackLength;
        public int initBeat; // At which beat do we init the cue?
        public int countdown; // How many beats countdown is there?
        public int startCueLogic; // At which beat do we start the cue logic?
        public int beatToMute; // At which beat do we mute the instrument?
        public Instrument instrument; // Which instrument are we talking about?
    };

    List<CueInfo> m_cueInfos;

    [SerializeField]
    AudioManager m_audioManager;

    [SerializeField] // How many beats before the actual beat do we notify the user?
    int m_cueCountdown = 8;

    Metronome m_metronome;

    int m_score;
    int m_maxScore;

    [SerializeField]
    private Text m_scoreText;

    [SerializeField]
    private GameObject m_scoreUpdateFeedbackPrefab;
}
