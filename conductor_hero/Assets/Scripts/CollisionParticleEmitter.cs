using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionParticleEmitter : MonoBehaviour {
    public float lifeTime = 0.25f;

    void Start()
    {
        StartCoroutine(DestroyRoutine(lifeTime));
    }

    private IEnumerator DestroyRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
}
