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

    [Header("Other Steam Things")]
    public GameObject m_HMD;

    [Header("Conductor Simulator Related")]
    public Metronome m_metronome;
    public GameObject m_particlePrefab;

    public GameObject m_sphereContainer;
    private List<MotionTrackerSphere> m_trackerSpheres;

    private const int NUM_BEATS_PER_MEASURE = 4;

    public int nextSphereIndex
    {
        get
        {
            return m_nextSphereIndex;
        }
        set
        {
            // Wraps around for simplicity
            if(value > NUM_BEATS_PER_MEASURE - 1)
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
    private Transform m_targetTransform;

    private void Start()
    {
        m_leftControllerPointer.PointerIn += new PointerEventHandler(OnPointerIn);
        m_leftControllerPointer.PointerOut += new PointerEventHandler(OnPointerOut);
        m_metronome.onBeatTickedCallback += MetronomeCallback;
        m_trackerSpheres = new List<MotionTrackerSphere>(m_sphereContainer.GetComponentsInChildren<MotionTrackerSphere>());
    }

    private void Update()
    {
        // Input Handling
    }

    public void OnSphereCollision(int sphereIndex, MotionTrackerSphere sphere)
    {
        if(sphereIndex == m_nextSphereIndex)
        {
            // Quality of being on beat could be decided in GameManager script I guess?
            // Give some visual feedback for testing purposes. 
            // Excellent! Good. Miss. or something
            var collisionToBeatDifference = m_metronome.OnBeat();
            Instantiate(m_particlePrefab, sphere.transform);

            // This will be reset if we are too late currently. 
            // Quickfix for being able to hit a bit before beat
            nextSphereIndex++;
        }
    }

    // Gets called on every metronome beat. 
    public void MetronomeCallback(int beatID)
    {
        nextSphereIndex = (beatID % NUM_BEATS_PER_MEASURE) + 1;

        // Update visuals
        foreach (var sphere in m_trackerSpheres)
        {
            if (sphere.m_SphereIndex == nextSphereIndex)
            {
                sphere.m_meshRenderer.material = sphere.m_nextInOrderMaterial;
            }
            else
            {
                sphere.m_meshRenderer.material = sphere.m_defaultMaterial;
            }
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

    #region PointedObjectCallbacks
    // TODO: Rename this, but first, find a better name
    private void OnPointerIn(object o, PointerEventArgs e)
    {
        m_targetTransform = e.target.transform;
    }

    private void OnPointerOut(object o, PointerEventArgs e)
    {
        m_targetTransform = null;
    }
    #endregion
}
