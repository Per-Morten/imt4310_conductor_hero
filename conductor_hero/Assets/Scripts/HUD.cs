using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{

    [SerializeField]
    private AudioManager m_audiomanager;
    private float volume;

    [SerializeField]
    private float m_fillAmount;
    [SerializeField]
    private Image m_view;

    // Use this for initialization
    void Start()
    {
        m_view = GameObject.Find("volumeFG").GetComponent<Image>();
        volume = m_audiomanager.GetInstrumentVolume(m_audiomanager.GetFirstInstrument());
        m_fillAmount = volume;
        UpdateView();
    }

    void Update()
    {
        m_fillAmount = Mathf.Clamp(m_audiomanager.GetInstrumentVolume(m_audiomanager.GetFirstInstrument()), 0.0f, 1.0f);
        UpdateView();
    }

    private void UpdateView()
    {
        m_view.fillAmount = m_fillAmount;
    }
}
