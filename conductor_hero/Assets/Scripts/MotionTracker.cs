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

    [SerializeField]
    private Transform m_targetTransform;

    private int m_nextSphereIndex = 0;
    private const int MAX_INDICES = 3;

    // NOTE: What should we do if you somehow lose your beat?
    //       Reset m_nextSphereIndex to 0 at the end of every 4th beat?
    public void OnSphereCollision(int sphereIndex, MotionTrackerSphere sphere)
    {
        if(sphereIndex == m_nextSphereIndex)
        {
            IncrementNextSphereIndex();
            sphere.m_meshRenderer.material = sphere.m_onRightOrderMaterial;
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

    // TODO: Rename this, but first, find a better name
    private void OnPointerIn(object o, PointerEventArgs e)
    {
        m_targetTransform = e.target.transform;
    }

    private void OnPointerOut(object o, PointerEventArgs e)
    {
        m_targetTransform = null;
    }

    private void Start()
    {
        m_leftControllerPointer.PointerIn += new PointerEventHandler(OnPointerIn);
        m_leftControllerPointer.PointerOut += new PointerEventHandler(OnPointerOut);
    }

    private void Update()
    {
        // Input Handling
    }
}
