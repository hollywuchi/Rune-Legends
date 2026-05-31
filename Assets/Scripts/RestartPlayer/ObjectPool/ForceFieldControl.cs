using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldControl : MonoBehaviour
{
    private ParticleSystemForceField forceField;
    private CircleCollider2D coll;

    void Awake()
    {
        forceField = GetComponent<ParticleSystemForceField>();
        coll = GetComponent<CircleCollider2D>();
    }
    void OnEnable()
    {
        coll.enabled = false;
        forceField.gravity = 0f;
        StartCoroutine(ForvcChange());
    }

    private IEnumerator ForvcChange()
    {
        yield return new WaitForSeconds(2f);
        coll.enabled = true;
        forceField.gravity = 10f;
    }
}
