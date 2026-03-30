using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    public PlayerState CurrentState { get; private set; }

    // 初始化状态机（游戏开始时调用）
    public void Initialize(PlayerState startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    // 切换状态
    public void ChangeState(PlayerState newState)
    {
        CurrentState.Exit();        // 退出当前状态（比如清除冲刺特效）
        // StartCoroutine(CurrentState.SwitchAnimator(nextAnimation));  // 实现小动作的动态转换
        CurrentState = newState; // 替换状态
        CurrentState.Enter();    // 进入新状态（比如播放跳跃动画）
    }

    /// <summary>
    /// 尝试平滑过度到下一个动画
    /// </summary>
    /// <param name="nextAnimation"></param>
    public void SmoothAni(string nextAnimation)
    {
        StartCoroutine(CurrentState.SwitchAnimation(nextAnimation));
    }
}
