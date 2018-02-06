using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClicker : MonoBehaviour
{
    [SerializeField]
    Metronome met;
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
            //if (timing <= -0.05f)
            //    Debug.Log(string.Format("To early {0}", timing));
            //else if (timing >= 0.05f)
            //    Debug.Log(string.Format("To late {0}", timing));
            //else
            //    Debug.Log(string.Format("On Beat {0}", timing));

        }
    }
}
