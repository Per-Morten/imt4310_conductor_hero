using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClicker : MonoBehaviour
{
    [SerializeField]
    Metronome met;

    [SerializeField]
    GameManager gm;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            var timing = met.OnBeat();

            if (timing < -0.09f)
            {
                gm.AddScore(-3);
                Debug.Log(string.Format("Closer to next beat {0}", timing));
            }
            else if(timing > 0.09f)
            {
                gm.AddScore(-3);
                Debug.Log(string.Format("Closer to previous beat {0}", timing));
            }
            else
            {
                gm.AddScore(10);
                Debug.Log(string.Format("On beat{0}", timing));
            }
        }
    }
}
