using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Progressbar : MonoBehaviour
{

    [SerializeField]
    private float m_fillAmount;
    [SerializeField]
    private Image m_view;


    // Use this for initialization
    void Start()
    {
        m_fillAmount = 0.0f;
    }

   

    // Update is called once per frame
    void Update()
    {
       
        if (m_fillAmount < 0.8f)
        {
            m_fillAmount = m_fillAmount + 0.1f * Time.deltaTime;  
            VisuallyUpdateProgress();
            print(m_fillAmount);
        }
    }

    //get.length

    private void VisuallyUpdateProgress()
    {
        m_view.fillAmount = m_fillAmount;
    }

}
