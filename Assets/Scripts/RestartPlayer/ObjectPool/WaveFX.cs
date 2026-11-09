using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class WaveFX : MonoBehaviour
{
    private readonly float waveTime = 0.2f;
    private Material material;
    private static readonly int WaveDistenceFromCenter = Shader.PropertyToID("_WaveDistenceFromCenter");

    private Effect_Elements el;
    private void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;
        el = GetComponent<Effect_Elements>();

    }

    private void OnEnable()
    {
        StartCoroutine(Wave(-0.1f, 0.5f));
    }

    private IEnumerator Wave(float startPoint,float endPoint)
    {
        material.SetFloat(WaveDistenceFromCenter,startPoint);

        float waveTimer = 0;

        while (waveTimer < waveTime)
        {
            waveTimer += Time.deltaTime;

            var waveDistance = Mathf.Lerp(startPoint, endPoint, (waveTimer / waveTime));
            material.SetFloat(WaveDistenceFromCenter, waveDistance);
            yield return null;
        }
        
        if(el != null)
            el.ReleasePool();
    }
}
