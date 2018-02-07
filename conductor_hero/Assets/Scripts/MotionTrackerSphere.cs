using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionTrackerSphere : MonoBehaviour
{
    [SerializeField]
    private Material m_defaultMaterial;
    [SerializeField]
    private Material m_onTriggerMaterial;

    [SerializeField]
    private int m_SphereIndex = 0;

    private MeshRenderer m_meshRenderer;

    // Use this for initialization
    void Start()
    {
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_meshRenderer.material = m_defaultMaterial;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        m_meshRenderer.material = m_onTriggerMaterial;
    }

    private void OnTriggerExit(Collider other)
    {
        m_meshRenderer.material = m_defaultMaterial;
    }
}
