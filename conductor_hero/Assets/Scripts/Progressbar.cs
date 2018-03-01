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


    // Use this for initialization
    void Start()
    {
        m_fillAmount = 0.0f;

        m_songlength = m_audiomanager.GetSonglength();
        print(m_songlength);

        time = m_audiomanager.GetCurrentPlaybackPosition();
        print(time);
        m_audiomanager.PlayMusic(0);
        // 200 - 0* (1 - 0) / 

        //m_songLength = m_songLength / 100; //normalize

    }


    // Update is called once per frame
    void Update()
    {
        if (m_fillAmount < 1f)
        {
            float tick = 238 * 0.01f;
            m_fillAmount += m_fillAmount + tick;
                
            //m_fillAmount + 0.1f * Time.deltaTime;  
            VisuallyUpdateProgress();



         }
    }



   

    private void VisuallyUpdateProgress()
    {
        m_view.fillAmount = m_fillAmount;
    }

}
