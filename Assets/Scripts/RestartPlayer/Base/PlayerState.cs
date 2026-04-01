using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public abstract class PlayerState
{
    protected Player player;
    protected PlayerStateMachine stateMachine;
    protected bool isAnimationFinished;

    /// <summary>
    /// 构造函数,自动赋值
    /// </summary>
    /// <param name="player"></param>
    /// <param name="stateMachine"></param>
    public PlayerState(Player player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter()
    {
        isAnimationFinished = false;
    }             // 进入状态的一瞬间执行
    public virtual void LogicUpdate() { }       // 每一帧执行（Update）
    public virtual void PhysicsUpdate() { }     // 物理帧执行（FixUpdate)
    public virtual void Exit() { }              // 离开状态的一瞬间执行
    public virtual void OnAnimationFinished()
    {
        isAnimationFinished = true;
    }    
}
