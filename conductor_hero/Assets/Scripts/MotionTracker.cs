using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionTracker : MonoBehaviour
{
    [Header("Left Controller Components")]
    public SteamVR_TrackedController leftControllerTracking;
    public SteamVR_LaserPointer leftControllerPointer;

    [Header("Right Controller Components")]
    public SteamVR_TrackedController rightControllerTracking;

    [Header("Other")]
    public GameObject prefabToInitiateOnRightTrigger;
    public GameObject HMD;

    [SerializeField]
    private Transform targetTransform;

    // TODO: Rename this, but first, find a better name
    private void OnPointerIn(object o, PointerEventArgs e)
    {
        targetTransform = e.target.transform;
    }

    private void OnPointerOut(object o, PointerEventArgs e)
    {
        targetTransform = null;
    }

    private void Start()
    {
        leftControllerPointer.PointerIn += new PointerEventHandler(OnPointerIn);
        leftControllerPointer.PointerOut += new PointerEventHandler(OnPointerOut);
    }

    void Update()
    {
        // Input handling
        if (rightControllerTracking.triggerPressed)
        {
            Instantiate(prefabToInitiateOnRightTrigger, rightControllerTracking.transform.position, Quaternion.identity);
        }

        if (rightControllerTracking.menuPressed)
        {
            var overlappingColliders = Physics.OverlapSphere(rightControllerTracking.transform.position, 0.1f);

            foreach (var collider in overlappingColliders)
            {
                if (collider.CompareTag("DestroyableObject"))
                    Destroy(collider.gameObject);
            }
        }

        if (leftControllerTracking.menuPressed)
        {
            HMD.transform.position = new Vector3(targetTransform.position.x, HMD.transform.position.y, targetTransform.position.z);
        }
    }
}
