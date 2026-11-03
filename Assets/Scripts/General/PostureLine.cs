using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PostureLine : MonoBehaviour
{
    public SpriteRenderer yelloLine;
    public Character character;

    public SpriteRenderer spriteRenderer_Line;
    public SpriteRenderer spriteRenderer_Full;

    private EventBinding<PoseChangeEvent> poseChangeEventBinding;

    void Awake()
    {
        if (spriteRenderer_Full != null) spriteRenderer_Full.color = new Color(1, 1, 1, 0);
        if (spriteRenderer_Line != null) spriteRenderer_Line.color = new Color(1, 1, 1, 0);
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

    void OnDestroy()
    {
        if (yelloLine != null) yelloLine.transform.DOKill();
        if (spriteRenderer_Full != null) spriteRenderer_Full.DOKill();
        if (spriteRenderer_Line != null) spriteRenderer_Line.DOKill();
    }

    private void ChangeLine(PoseChangeEvent e)
    {
        if (e.character != character || yelloLine == null) return;
        yelloLine.transform.DOKill();
        float targetPosture = e.targetPosture / e.maxPosture;

        if (e.duration > 0f)
        {
            yelloLine.transform.DOScaleX(targetPosture, e.duration);
        }
        else
        {
            yelloLine.transform.localScale = new Vector3(targetPosture, 1f, 1f);
        }

        if (targetPosture > 0.01f)
            Unfade();
        else fade();
    }

    private void fade()
    {
        if (spriteRenderer_Full != null)
        {
            spriteRenderer_Full.DOKill();
            spriteRenderer_Full.DOFade(0f, 0.4f);
        }
        if (spriteRenderer_Line != null)
        {
            spriteRenderer_Line.DOKill();
            spriteRenderer_Line.DOFade(0f, 0.4f);
        }
    }
    private void Unfade()
    {
        if (spriteRenderer_Full != null)
        {
            spriteRenderer_Full.DOKill();
            spriteRenderer_Full.DOFade(1f, 0.2f);
        }
        if (spriteRenderer_Line != null)
        {
            spriteRenderer_Line.DOKill();
            spriteRenderer_Line.DOFade(1f, 0.2f);
        }
    }
}
