using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PostureLine : MonoBehaviour
{
    public SpriteRenderer yelloLine;
    public Character character;
    public float currentPosture;

    private EventBinding<PoseChangeEvent> poseChangeEventBinding;

    void Awake()
    {
        // 初始化
        currentPosture = 0;
    }
    void OnEnable()
    {
        poseChangeEventBinding = new EventBinding<PoseChangeEvent>(ChangeLine);
        
        EventBus<PoseChangeEvent>.Register(poseChangeEventBinding);
    }

    void OnDisable()
    {
        EventBus<PoseChangeEvent>.Deregister(poseChangeEventBinding);
    }
    private void ChangeLine(PoseChangeEvent e)
    {
        if (e.character != character || yelloLine == null) return;
        yelloLine.transform.DOScaleX(e.targetPosture / e.maxPosture,e.duration);
    }
}
