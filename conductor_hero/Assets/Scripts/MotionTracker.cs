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

    [SerializeField]
    private float m_onBeatThreshold = 0.15f;

    public GameObject m_sphereContainer;
    private List<MotionTrackerSphere> m_trackerSpheres;

    [SerializeField]
    private GameManager m_gameManager;

    private const int NUM_BEATS_PER_MEASURE = 4;

    private const int MAX_INDICES = 3;
    private Transform m_targetTransform;

    private struct NextSphereIndex
    {
        public bool m_alreadyUpdated;
        public int nextSphereIndex
        {
            get
            {
                return m_nextSphereIndex;
            }
            set
            {
                // Wraps around for simplicity
                if (value > NUM_BEATS_PER_MEASURE - 1)
                {
                    m_nextSphereIndex = 0;
                }
                else
                {
                    m_nextSphereIndex = value;
                }
            }
        }

        // To separate our game logic from visual logic
        public int nextVisualSphereIndex
        {
            get
            {
                return m_nextVisualSphereIndex;
            }
            set
            {
                // Wraps around for simplicity
                if (value > NUM_BEATS_PER_MEASURE - 1)
                {
                    m_nextVisualSphereIndex = 0;
                }
                else
                {
                    m_nextVisualSphereIndex = value;
                }
            }
        }
        private int m_nextSphereIndex;
        private int m_nextVisualSphereIndex;
    }
    private NextSphereIndex m_nextSphereIndexStruct;

    [Header("Interpolation Related")]
    [SerializeField]
    private Transform m_trailRendererSphereTransform;

    [SerializeField]
    private float m_interpolationSpeed = 5;

    [SerializeField]
    private AnimationCurve m_interpolationVelocity;

    private Vector3 m_interpolationStartPos;
    private Vector3 m_interpolationEndPos;
    private float m_interpolationTimer = 0;


    private void Start()
    {
        m_leftControllerPointer.PointerIn += new PointerEventHandler(OnPointerIn);
        m_leftControllerPointer.PointerOut += new PointerEventHandler(OnPointerOut);
        m_metronome.onBeatTickedCallback += MetronomeCallback;
        m_trackerSpheres = new List<MotionTrackerSphere>(m_sphereContainer.GetComponentsInChildren<MotionTrackerSphere>());
        m_rightControllerTracking.MenuButtonUnclicked += new ClickedEventHandler(OnMenuRelease);
    }

    private void Update()
    {
        if (m_interpolationTimer <= 1)
        {
            m_interpolationTimer += Time.deltaTime * m_interpolationSpeed;
            m_trailRendererSphereTransform.position = Vector3.Lerp(m_interpolationStartPos, m_interpolationEndPos, m_interpolationVelocity.Evaluate(m_interpolationTimer));
        }

        if(m_rightControllerTracking.menuPressed)
        {
            var oldPosition = m_sphereContainer.transform.position;
            m_sphereContainer.transform.position = new Vector3(oldPosition.x, m_rightControllerTracking.transform.position.y, oldPosition.z);
        }
    }

    /// <summary>
    /// Gets called whenever the controller collides with a sphere.
    /// Checks whether we are on beat or not and instantiates visual feedback depending on whether this is on/off beat. 
    /// </summary>
    /// <param name="sphereIndex">The index of the sphere we have collided with</param>
    /// <param name="sphere">The actual sphere object we collided with</param>
    public void OnSphereCollision(int sphereIndex, MotionTrackerSphere sphere)
    {
        if(sphereIndex == m_nextSphereIndexStruct.nextSphereIndex)
        {
            var collisionToBeatDifference = m_metronome.OnBeat();

            // Are we on beat?
            if(collisionToBeatDifference <= m_onBeatThreshold && collisionToBeatDifference >= -m_onBeatThreshold)
            {
                // Give some visual feedback
                Instantiate(m_particlePrefab, sphere.transform);

                m_gameManager.AddScore(1);
            }
            else
            {
                // Give some negative visual feedback for missing the beat
                var particleObject = Instantiate(m_particlePrefab, sphere.transform);
                var mainSystem = particleObject.GetComponent<ParticleSystem>().main;
                mainSystem.startColor =  new Color(255, 0, 0, 1);
                Debug.LogFormat("CollisionToBeatDifference: {0}, BeatThreshold: {1}", collisionToBeatDifference, m_onBeatThreshold);
            }

            // This will be reset if we are too late currently. 
            // Quickfix for being able to hit a bit before beat
            m_nextSphereIndexStruct.nextSphereIndex = sphereIndex + 1;

            // We want to avoid cases where the index is reset back a value due to a callback when we were early
            m_nextSphereIndexStruct.m_alreadyUpdated = true;
        }
    }

    // Gets called on every metronome beat. 
    public void MetronomeCallback(int beatID)
    {
        var oldVisualIndex = m_nextSphereIndexStruct.nextVisualSphereIndex;
        m_nextSphereIndexStruct.nextVisualSphereIndex = (beatID % NUM_BEATS_PER_MEASURE) + 1;

        // Update visuals
        foreach (var sphere in m_trackerSpheres)
        {
            if (sphere.m_SphereIndex == m_nextSphereIndexStruct.nextVisualSphereIndex)
            {
                sphere.m_meshRenderer.material = sphere.m_nextInOrderMaterial;

                // Interpolation related
                var oldSphere = GetSphereAtIndex(oldVisualIndex);
                m_trailRendererSphereTransform.position = oldSphere.transform.position;
                UpdateInterpolationTargets(oldSphere.transform.position, GetSphereAtIndex(m_nextSphereIndexStruct.nextVisualSphereIndex).transform.position);
                sphere.GetComponentInChildren<ParticleSystem>().Play();
                
            }
            else
            {
                sphere.m_meshRenderer.material = sphere.m_defaultMaterial;
            }
        }

        // If the index already was updated through OnSphereCollision() we have no need to update
        if (m_nextSphereIndexStruct.m_alreadyUpdated)
        {
            m_nextSphereIndexStruct.m_alreadyUpdated = false;
        }
        else
        {
            m_nextSphereIndexStruct.nextSphereIndex = (beatID % NUM_BEATS_PER_MEASURE) + 1;
        }
    }

    private void UpdateInterpolationTargets(Vector3 start, Vector3 end)
    {
        m_interpolationStartPos = start;
        m_interpolationEndPos = end;
        m_interpolationTimer = 0;
    }

    private MotionTrackerSphere GetSphereAtIndex(int sphereIndex)
    {
        foreach(var sphere in m_trackerSpheres)
        {
            if(sphere.m_SphereIndex == sphereIndex)
            {
                return sphere;
            }
        }
        return null;
    }

    void OnMenuRelease(object sender, ClickedEventArgs e)
    {
        m_metronome.enabled = true;
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
