using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionTracker : MonoBehaviour
{
    [Header("Left Controller Components")]
    public SteamVR_TrackedController m_leftControllerTracking;
    public SteamVR_LaserPointer m_leftControllerPointer;

    [Header("Right Controller Components")]
    public SteamVR_TrackedController m_rightControllerTracking;

    [Header("Other")]
    public GameObject m_HMD;
    public Metronome m_metronome;

    [SerializeField]
    private Transform m_targetTransform;

    private const int NUM_BEATS = 4;

    public int nextSphereIndex
    {
        get
        {
            return m_nextSphereIndex;
        }
        set
        {
            // Wraps around for simplicity
            if(value > NUM_BEATS - 1)
            {
                m_nextSphereIndex = 0;
            }
            else
            {
                m_nextSphereIndex = value;
            }
        }
    }
    private int m_nextSphereIndex = 0;

    private const int MAX_INDICES = 3;
    
    public void OnSphereCollision(int sphereIndex, MotionTrackerSphere sphere)
    {
        if(sphereIndex == m_nextSphereIndex)
        {
            sphere.m_meshRenderer.material = sphere.m_nextInOrderMaterial;

            // TODO: Ask Metronome whether we're on beat
            // Quality of being on beat could be decided in GameManager script I guess?
            // Give some visual feedback for testing purposes. 
            // Excellent! Good. Miss. or something
        }
    }

    private void IncrementNextSphereIndex()
    {
        m_nextSphereIndex++;
        if (m_nextSphereIndex > MAX_INDICES)
        {
            m_nextSphereIndex = 0;
        }
    }

    private void Start()
    {
        m_leftControllerPointer.PointerIn += new PointerEventHandler(OnPointerIn);
        m_leftControllerPointer.PointerOut += new PointerEventHandler(OnPointerOut);
    }

    private void Update()
    {
        // Input Handling

        // Beat logic
        if (m_metronome.beatID - 1 != nextSphereIndex)
        {
            nextSphereIndex = m_metronome.beatID - 1;
        }
    }

    // TODO: Rename this, but first, find a better name
    private void OnPointerIn(object o, PointerEventArgs e)
    {
        m_targetTransform = e.target.transform;
    }

    private void OnPointerOut(object o, PointerEventArgs e)
    {
        m_targetTransform = null;
    }
}
