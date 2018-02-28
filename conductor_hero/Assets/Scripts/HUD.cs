using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

    public AudioManager m_audiomanager;
    private float volume;

    [SerializeField]
    private float m_fillAmount;
    [SerializeField]
    private Image m_view;

    // Use this for initialization
    void Start()
    {
        m_view = GameObject.Find("volumeFG").GetComponent<Image>();
        m_audiomanager = GameObject.Find("audiomanager").GetComponent<AudioManager>();
        volume = m_audiomanager.GetInstrumentVolume(m_audiomanager.GetFirstInstrument());
        m_fillAmount = volume;
        UpdateView();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
            DecreaseVolume();

        if (Input.GetMouseButton(1))
            IncreaseVolume();
       

    }

    public void IncreaseVolume()
    {
        if (m_fillAmount < 0.99f)
        {
            volume = volume + 0.004f;
            m_fillAmount = volume;
            UpdateView();
            m_audiomanager.SetInstrumentsVolume(volume);
        }
    }

    public void DecreaseVolume()
    {
        if (m_fillAmount > 0.0f)
        {
            volume = volume - 0.004f;
            m_fillAmount = volume;
            UpdateView();
            m_audiomanager.SetInstrumentsVolume(volume);
        }
    }

    private void UpdateView()
    {
        m_view.fillAmount = m_fillAmount;
        Debug.Log(string.Format("volume: {0}", volume));
    }



   
   
}
