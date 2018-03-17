using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatBounce : MonoBehaviour {
    public Metronome m_metronome;

    [SerializeField]
    private float m_bounceSpeed = 5;

    [SerializeField]
    private float m_bounceHeight = 5;

    [SerializeField]
    private AnimationCurve m_bounceVelocity;

    private float m_interpolationTimer = 0;
    private bool m_animationActive = false;

    private Vector3 m_defaultPosition;
    private Vector3 m_bouncePosition;

    private bool m_playAnimation;

    public void StartBouncing()
    {
        m_playAnimation = true;
    }

    public void StopBouncing()
    {
        m_playAnimation = false;
        transform.position = m_defaultPosition;
    }

    private void Awake()
    {
        m_metronome = GameObject.Find("Metronome").GetComponent<Metronome>();
        m_metronome.onBeatTickedCallback += OnBeatCallback;
        m_defaultPosition = transform.position;
        m_bouncePosition = new Vector3(m_defaultPosition.x, m_defaultPosition.y + m_bounceHeight, m_defaultPosition.z);
    }

    private void Update()
    {
        if(m_playAnimation && m_animationActive)
        {
            m_interpolationTimer += Time.deltaTime * m_bounceSpeed;
            var sineTimer = Mathf.Sin(m_interpolationTimer);
            transform.position = Vector3.Lerp(m_defaultPosition, m_bouncePosition, m_bounceVelocity.Evaluate(sineTimer));
            if (sineTimer <= 0)
            {
                m_animationActive = false;
                transform.position = m_defaultPosition;
            }
        }
    }

    private void OnBeatCallback(int beatID)
    {
        m_animationActive = true;
        m_interpolationTimer = 0;
    }
}
