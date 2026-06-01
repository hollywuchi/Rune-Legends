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
        if (s.ctx.CurrentFocus < s.config.LightCrownFocusCost)
        {
            isAnimFinished = true;
            return;
        }

        // 检查是否达到最大叠加层数
        if (s.character.attackBuffStack >= s.config.maxAttackBuffStack)
        {
            isAnimFinished = true;
            return;
        }

        // 设置Buff状态
        s.ctx.IsBuffing = true;
        s.anim.SetIsLightCrown(true);

        // 停止移动
        s.motor.SetVelocityX(0);
        s.inputGate.Freeze(0.2f);

        Debug.Log("进入攻击力Buff状态");
    }

    public override Transition LogicUpdate()
    {
        // 动画完成
        if (isAnimFinished)
        {
            return new Transition(PlayerStateId.Idle);
        }

        // Buff动画完成，施加Buff
        if (s.ctx.LightCrownPerformedThisFrame)
        {
            ApplyBuff();
            Debug.Log("攻击力Buff施加完成");
            if (isAnimFinished)
                return new Transition(PlayerStateId.Idle);
        }
        // 松开按键，取消Buff
        else if (!s.ctx.IsHoldingLightCrown)
        {
            Debug.Log("Buff被取消");
            return new Transition(PlayerStateId.Idle);
        }

        // 离地打断
        if (!s.ctx.IsGrounded)
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
        s.ctx.IsBuffing = false;
        s.ctx.LightCrownPerformedThisFrame = false;
        s.anim.SetIsLightCrown(false);
        Debug.Log("退出攻击力Buff状态");
    }

    /// <summary>
    /// 施加攻击力Buff
    /// </summary>
    private void ApplyBuff()
    {
        if (s.character == null) return;

        // 消耗专注值
        s.character.currentFocus -= s.config.LightCrownFocusCost;
        s.ctx.CurrentFocus = s.character.currentFocus;

        // 增加Buff层数
        s.character.attackBuffStack = Mathf.Min(
            s.character.attackBuffStack + 1,
            s.config.maxAttackBuffStack
        );

        // 更新配置中的倍率（可选，用于显示）
        s.character.attackBuffMultiplier = s.config.attackBuffMultiplier;

        // TODO：UI方面,通知UI更新
        s.character.OnHealthChange?.Invoke(s.character);

    }

    /// <summary>
    /// 动画事件回调：Buff动画完成
    /// </summary>
    public void OnBuffAnimFinished()
    {
        isAnimFinished = true;
        s.ctx.LightCrownPerformedThisFrame = true;
    }
}
