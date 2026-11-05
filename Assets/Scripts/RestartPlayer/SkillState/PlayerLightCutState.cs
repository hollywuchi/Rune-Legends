using RestartPlayer.HFSM;
using UnityEngine;

// using UnityEngine.UIElements;

/// <summary>
/// 霹雳一闪技能状态
/// 按住技能键蓄力，松开后向前冲刺并造成伤害
/// 消耗专注值
/// </summary>
public class PlayerLightCutState : PlayerState
{
    private bool isAnimFinished;

    public PlayerLightCutState(PlayerServices s) : base(s)
    {
    }

    public override void Enter()
    {
        base.Enter();
        isAnimFinished = false;

        // 检查专注值是否足够
        if (s.context.CurrentFocus < s.config.lightCutFocusCost)
        {
            isAnimFinished = true;
            return;
        }

        s.context.IsLightCutCharging = true;
        s.context.LightCutPerformedThisFrame = false;

        // 停止移动
        s.motor.SetVelocityX(0);


        s.anim.SetToCharging();
        s.anim.SetIsCharging(true);
    }

    public override Transition LogicUpdate()
    {
        // 受伤状态检查（优先级最高）
        if (s.context.IsHurt)
        {
            return new Transition(PlayerStateId.Hurt);
        }

        // 动画完成，返回
        if (isAnimFinished)
        {
            return new Transition(PlayerStateId.Idle);
        }


        // 蓄力阶段：松开按键则释放技能
        if (!s.context.IsHoldingLightCut && !s.context.IsLightCutting && s.context.LightCutPerformedThisFrame)
        {
            ReleaseLightCut();
            s.context.IsLightCutting = true;
        }

        // 打断条件：在蓄力阶段（未冲刺），松开按键，且蓄力未完成
        if (!s.context.IsHoldingLightCut && !s.context.IsLightCutting && !s.context.LightCutPerformedThisFrame)
        {
            return new Transition(PlayerStateId.Idle);
        }

        // 离地则中断
        if (!s.context.IsGrounded && !isAnimFinished)
        {
            return new Transition(PlayerStateId.Fall);
        }

        return Transition.None;
    }

    public override void PhysicsUpdate()
    {
        // 蓄力期间保持静止
        if (!s.context.IsLightCutting)
        {
            s.motor.SetVelocityX(0);
        }
    }

    public override void Exit()
    {
        base.Exit();
        if (s.context.IsLightCutCharging)
        {
            s.anim.SetExitCharging();
            EventBus<ReliseFxEvent>.Raise(new ReliseFxEvent());
        }
        s.context.IsLightCutCharging = false;
        s.context.LightCutPerformedThisFrame = false;
        s.context.IsLightCutting = false;
        s.context.IsHoldingLightCut = false;
        s.motor.IgnorePlayerCollision(s.context.EnemyLayer, false);
    }

    /// <summary>
    /// 释放霹雳一闪
    /// </summary>
    private void ReleaseLightCut()
    {
        s.anim.SetIsCharging(false);

        s.context.IsLightCutCharging = false;
        s.context.LightCutPerformedThisFrame = false;
        s.motor.IgnorePlayerCollision(s.context.EnemyLayer, true); // 冲刺期间无敌

        // 消耗专注值
        s.character.currentFocus -= s.config.lightCutFocusCost;
        s.context.CurrentFocus = s.character.currentFocus;
        EventBus<FocusChangeEvent>.Raise(new FocusChangeEvent { character = s.character });
    }

    public void OnLightCutAnimFinished()
    {
        isAnimFinished = true;
        s.context.IsLightCutting = false;
    }

    public void LightCutMove(float cutForce)
    {
        s.motor.LightCutDash(s.context.FacingDirection, cutForce);
    }
}
