using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionTrackerSphere : MonoBehaviour
{
    public MotionTracker m_motionTrackerReference; 

    public Material m_defaultMaterial;
    public Material m_onTriggerMaterial;
    public Material m_onRightOrderMaterial;

    public MeshRenderer m_meshRenderer;

    [SerializeField]
    private int m_SphereIndex = 0;

    [SerializeField][Range(0, 100)]
    private int m_volumeLevel = 100;

    // Use this for initialization
    void Start()
    {
        m_meshRenderer.material = m_defaultMaterial;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ConductorBaton"))
        {
            m_meshRenderer.material = m_onTriggerMaterial;
            m_motionTrackerReference.OnSphereCollision(m_SphereIndex, this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ConductorBaton"))
        {
            m_meshRenderer.material = m_defaultMaterial;
        }
    }
}
