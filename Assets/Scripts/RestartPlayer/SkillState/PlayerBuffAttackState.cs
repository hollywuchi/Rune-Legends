using RestartPlayer.HFSM;
using UnityEngine;

/// <summary>
/// 攻击力Buff技能状态
/// 长按技能键施加Buff，消耗专注值增加攻击力
/// 可叠加，有最大层数限制
/// </summary>
public class PlayerLightCrownState : PlayerState
{
    private bool isAnimFinished;

    public PlayerLightCrownState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        isAnimFinished = false;

        // 检查专注值是否足够
        if (s.context.CurrentFocus < s.config.LightCrownFocusCost)
        {
            isAnimFinished = true;
            return;
        }

        // 检查是否达到最大叠加层数
        if (s.focusSystem.attackBuffStack >= s.config.maxAttackBuffStack)
        {
            isAnimFinished = true;
            return;
        }

        // 设置Buff状态
        s.context.IsBuffing = true;
        s.anim.SetIsLightCrown(true);

        // 停止移动
        s.motor.SetVelocityX(0);

    }

    public override Transition LogicUpdate()
    {
        // 受伤状态检查（优先级最高）
        if (s.context.IsHurt)
        {
            // 受伤打断Buff，返回受伤状态
            return new Transition(PlayerStateId.Hurt);
        }

        // 动画完成
        if (isAnimFinished)
        {
            return new Transition(PlayerStateId.Idle);
        }

        // Buff动画完成，施加Buff
        if (s.context.LightCrownPerformedThisFrame)
        {
            ApplyBuff();
            if (isAnimFinished)
                return new Transition(PlayerStateId.Idle);
        }
        // 松开按键，取消Buff
        else if (!s.context.IsHoldingLightCrown)
        {
            return new Transition(PlayerStateId.Idle);
        }

        // 离地打断
        if (!s.context.IsGrounded)
        {
            return new Transition(PlayerStateId.Fall);
        }

        return Transition.None;
    }

    public override void PhysicsUpdate()
    {
        // Buff时保持静止
        s.motor.SetVelocityX(0f);
    }

    public override void Exit()
    {
        base.Exit();
        s.context.IsBuffing = false;
        s.context.LightCrownPerformedThisFrame = false;
        s.anim.SetIsLightCrown(false);
        Debug.Log("退出攻击力Buff状态");
    }

    /// <summary>
    /// 施加攻击力Buff
    /// </summary>
    private void ApplyBuff()
    {
        if (s.character == null) return;

        EventBus<FocusChangeEvent>.Raise(new FocusChangeEvent
            { isGetFous = false, changeFocusCount = s.config.LightCrownFocusCost });
    }

    /// <summary>
    /// 动画事件回调：Buff动画完成
    /// </summary>
    public void OnBuffAnimFinished()
    {
        isAnimFinished = true;
        s.context.LightCrownPerformedThisFrame = true;
    }
}
