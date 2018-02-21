using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionParticleEmitter : MonoBehaviour {
    public float lifeTime = 0.0f;

    void Start()
    {
        if(lifeTime == 0.0f)
        {
            lifeTime = GetComponent<ParticleSystem>().main.startLifetime.constant;
        }
        StartCoroutine(DestroyRoutine(lifeTime));
    }

    private IEnumerator DestroyRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
}
