using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Scripting.APIUpdating;

public class SpriteGroupAlphaController : MonoBehaviour
{
    private SpriteRenderer[] renders;
    private float currentAlpha;

    [Header("玩家组件")]
    public Transform playerTrans;

    [Header("淡入淡出的速度")]
    public float fadeOutSpeed = 1f;
    public float fadeInSpeed = 1f;

    private float currentTargetAlpha = 1f;
    private bool playerInside = false;
    private float lastPlayerPosX;

    [Header("判断玩家到底进入了那个区域")]
    public AreaName areaName;
    void Awake()
    {
        renders = GetComponentsInChildren<SpriteRenderer>();
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerTrans = collision.transform;
            playerInside = true;
            lastPlayerPosX = playerTrans.position.x;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInside = false;
        }
    }

    void Update()
    {
        if (playerInside && playerTrans != null)
        {
            DetectPlayerMove();
        }

        SetAlpha(currentTargetAlpha);
    }

    /// <summary>
    /// 判断玩家移动方向
    /// </summary>
    private void DetectPlayerMove()
    {
        float movementDir = playerTrans.position.x - lastPlayerPosX;

        if (areaName == AreaName.Forest)
        {
            if (movementDir >= 0.001f)
                currentTargetAlpha -= fadeOutSpeed * Time.deltaTime;
            else if (movementDir <= -0.001f)
                currentTargetAlpha += fadeInSpeed * Time.deltaTime;
        }

        if (areaName == AreaName.Desert)
        {
            if (movementDir >= 0.001f)
                currentTargetAlpha += fadeInSpeed * Time.deltaTime;
            else if (movementDir <= -0.001f)
                currentTargetAlpha -= fadeOutSpeed * Time.deltaTime;
        }

        lastPlayerPosX = playerTrans.position.x;
    }

    /// <summary>
    /// 设置当前Alpha值
    /// </summary>
    /// <param name="alpha"></param>
    void SetAlpha(float alpha)
    {
        if (renders == null) return;

        currentAlpha = Mathf.Clamp01(alpha);

        foreach (SpriteRenderer sp in renders)
        {
            if (sp != null)
            {
                Color color = sp.color;
                color.a = currentAlpha;
                sp.color = color;
            }
        }

    }
}
