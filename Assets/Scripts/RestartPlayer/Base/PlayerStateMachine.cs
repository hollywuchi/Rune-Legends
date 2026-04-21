using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine
{
    public PlayerState currentState { get; private set; }

    // 初始化状态机（游戏开始时调用）
    private Coroutine currentCoroutine;
    public void Initialize(PlayerState startingState)
    {
        currentState = startingState;
        currentState.Enter();
    }

    // 切换状态
    public void ChangeState(PlayerState newState)
    {
        currentState.Exit();        // 退出当前状态（比如清除冲刺特效）
        // StartCoroutine(CurrentState.SwitchAnimator(nextAnimation));  // 实现小动作的动态转换
        currentState = newState; // 替换状态
        currentState.Enter();    // 进入新状态（比如播放跳跃动画）
    }
}
