using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;
using UnityEngine.Events;

// 变相的EventHandler
[CreateAssetMenu(menuName = "Event/CharacterEventSO")]
public class CharacterEventSo : ScriptableObject
{
    // 声明一个事件，“触发事件”？
    public UnityAction<Character> OnEvenetRaised;

    // 事件的调用
    public void RaisedEvent(Character character)
    {
        OnEvenetRaised?.Invoke(character);
    }

    public UnityAction<PlayerController> OnRushRaised;
    public void _OnRushRaised(PlayerController playerController)
    {
        OnRushRaised?.Invoke(playerController);
    }
}