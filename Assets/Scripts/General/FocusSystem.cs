using System;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;


public class FocusSystem : MonoBehaviour
{
    public PlayerConfig config;
    
    [Header("当前专注值")]
    public float currentFocus;
    private float maxFocus;

    [Header("攻击buff层数")]
    public int attackBuffStack;
    public int attackBuffMultiplier;

    private EventBinding<FocusChangeEvent> focusChangeEventBinding;
    
    private void Awake()
    {
        maxFocus = config.maxFocus;
    }

    private void OnEnable()
    {
        focusChangeEventBinding = new EventBinding<FocusChangeEvent>(FocusChange);

        EventBus<FocusChangeEvent>.Register(focusChangeEventBinding);
    }

    private void OnDisable()
    {
        EventBus<FocusChangeEvent>.Deregister(focusChangeEventBinding);
    }
    
    /// <summary>
    /// 专注值增减方法
    /// </summary>
    /// <param name="e"></param>
    private void FocusChange(FocusChangeEvent e)
    {
        if (e.isGetFous)
        {
            if(currentFocus >= maxFocus) return;
            currentFocus = Mathf.Min(currentFocus + e.changeFocusCount,maxFocus);
        }
        else
        {
            if(currentFocus < 0) return;
            currentFocus = Mathf.Max(0, currentFocus - e.changeFocusCount);
        }
        
        // 修改完当前值之后顺手同步UI
        EventBus<FocusUIChangeEvent>.Raise(new FocusUIChangeEvent()
            { currentFocus = currentFocus, maxFocus = maxFocus });
    }
}