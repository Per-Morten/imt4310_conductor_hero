using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatLightInterpolation : MonoBehaviour {
    public Light lightSource;
    public Metronome metronome;

    [SerializeField]
    private float m_fadeSpeed = 1;

    [SerializeField]
    private AnimationCurve m_fadeVelocity;

    private float m_baseIntensity;
    private float m_interpTimer = 0;

    void Start()
    {
        m_baseIntensity = lightSource.intensity;
        metronome.onBeatTickedCallback += OnBeatLight;
    }

    void Update()
    {
        m_interpTimer += m_fadeSpeed * Time.deltaTime;
        lightSource.intensity = Mathf.Lerp(m_baseIntensity, 0, m_fadeVelocity.Evaluate(m_interpTimer));
    }

    private void OnBeatLight(int updatedBeatID)
    {
        m_interpTimer = 0;
        lightSource.intensity = m_baseIntensity;
    }


}
