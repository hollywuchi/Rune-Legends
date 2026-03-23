using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/VoidEventSo")]
// 这是一个没有返回值的事件，所有没有返回值的事件都可以往这里塞
// 本质上更像是一个无返回事假管理器
public class VoidSo : ScriptableObject
{
    public UnityAction OnEventRaised;

    // Raise /reiz/ 增加
    public void RaiseEvent()
    {
        OnEventRaised?.Invoke();
    }
}
