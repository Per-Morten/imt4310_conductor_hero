using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUpdateInterpolation : MonoBehaviour {
    [SerializeField]
    private float m_speed;

    [SerializeField]
    private float m_lifeTime;

    [SerializeField]
    private float m_yOffset = 10;

    [SerializeField]
    private float m_xOffset = -10;

    [SerializeField]
    private AnimationCurve m_fadeCurve;

    private float m_lerpTimer = 0;
    private RectTransform m_rectTransform;
    private Vector3 m_basePosition;
    private Text m_text;

    private void Start()
    {
        m_rectTransform = GetComponent<RectTransform>();
        m_basePosition = m_rectTransform.localPosition;
        m_rectTransform.localPosition = new Vector3(m_basePosition.x + m_xOffset, m_basePosition.y, m_basePosition.z);
        m_basePosition = m_rectTransform.localPosition;
        m_text = GetComponent<Text>();

        Invoke("Cleanup", m_lifeTime);
    }

    private void Update()
    {
        m_lerpTimer += Time.deltaTime * m_speed;
        m_rectTransform.localPosition = Vector3.Lerp(m_basePosition, 
                                                     m_basePosition + new Vector3(0, m_yOffset, 0), 
                                                     m_lerpTimer);

        m_text.color = new Color(m_text.color.r, m_text.color.g, m_text.color.b, Mathf.Lerp(1, 0, m_fadeCurve.Evaluate(m_lerpTimer)));
    }

    private void Cleanup()
    {
        Destroy(gameObject);
    }
}
