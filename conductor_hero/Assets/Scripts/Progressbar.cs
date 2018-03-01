using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Progressbar : MonoBehaviour
{

    public AudioManager m_audiomanager;

    [SerializeField]
    private float m_fillAmount;
    [SerializeField]
    private Image m_view;

    private float m_songlength; // = max value
    private int m_min = 0;
    private float time;
    float tick_counter = 0;

    // Use this for initialization
    void Start()
    {
        m_fillAmount = 0.0f;
        m_audiomanager = GameObject.Find("audiomanager").GetComponent<AudioManager>();
        m_songlength = m_audiomanager.GetSonglength();
        m_audiomanager.PlayMusic(0);
    }


    // Update is called once per frame
    void Update()
    {
        if (m_fillAmount < 1f)
        {
            float tick = (m_songlength * 0.01f)/4;
            
            time = m_audiomanager.GetCurrentPlaybackPosition();
            //Debug.Log(string.Format("Time {0}, tick {1}, Fill amount {2}, tick_counter {3}", time, tick, m_fillAmount, tick_counter));
            if (time >= tick_counter)
            {
                m_fillAmount = m_fillAmount + (0.01f)/4;
                tick_counter += tick;
            }           
            VisuallyUpdateProgress();
         }
    }

    private void VisuallyUpdateProgress()
    {
        m_view.fillAmount = m_fillAmount;
    }

}
