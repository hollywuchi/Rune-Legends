using RestartPlayer.HFSM;
using UnityEngine;

/// <summary>
/// 霹雳一闪技能状态
/// 按住技能键蓄力，松开后向前冲刺并造成伤害
/// 消耗专注值
/// </summary>
public class PlayerLightCutState : PlayerState
{
    private bool isAnimFinished;

    public PlayerLightCutState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        isAnimFinished = false;

        // 检查专注值是否足够
        if (s.ctx.CurrentFocus < s.config.lightCutFocusCost)
        {
            isAnimFinished = true;
            return;
        }

        s.ctx.IsLightCutCharging = true;

        // 停止移动
        s.motor.SetVelocityX(0);

        // 冻结移动输入
        s.inputGate.Freeze(0.2f);
        s.anim.SetToCharging();
        s.anim.SetIsCharging(true);
    }

    public override Transition LogicUpdate()
    {
        // 受伤状态检查（优先级最高）
        if (s.ctx.IsHurt)
        {
            // 受伤打断蓄力，返回受伤状态
            s.fxSpeaker.ReliseFX();
            return new Transition(PlayerStateId.Hurt);
        }

        // 动画完成，返回
        if (isAnimFinished)
        {
            return new Transition(PlayerStateId.Idle);
        }


        // 蓄力阶段：松开按键则释放技能
        if (!s.ctx.IsHoldingLightCut && !s.ctx.IsLightCutting && s.ctx.LightCutPerformedThisFrame)
        {
            ReleaseLightCut();
            s.ctx.IsLightCutting = true;
        }

        // 打断条件：在蓄力阶段（未冲刺），松开按键，且蓄力未完成
        if (!s.ctx.IsHoldingLightCut && !s.ctx.IsLightCutting && !s.ctx.LightCutPerformedThisFrame)
        {
            s.fxSpeaker.ReliseFX();
            s.anim.SetExitCharging();
            return new Transition(PlayerStateId.Idle);
        }

        // 离地则中断
        if (!s.ctx.IsGrounded && !isAnimFinished)
        {
            return new Transition(PlayerStateId.Fall);
        }

        return Transition.None;
    }

    public override void PhysicsUpdate()
    {
        // 蓄力期间保持静止
        if (!s.ctx.IsLightCutting)
        {
            s.motor.SetVelocityX(0);
        }
    }

    public override void Exit()
    {
        base.Exit();
        s.ctx.IsLightCutCharging = false;
        s.ctx.LightCutPerformedThisFrame = false;
        s.ctx.IsLightCutting = false;
        s.ctx.IsHoldingLightCut = false;
        s.motor.IgnorePlayerCollision(s.ctx.enemyLayer, false);
    }

    /// <summary>
    /// 释放霹雳一闪
    /// </summary>
    private void ReleaseLightCut()
    {
        s.anim.SetIsCharging(false);

        s.ctx.IsLightCutCharging = false;
        s.ctx.LightCutPerformedThisFrame = false;
        s.motor.IgnorePlayerCollision(s.ctx.enemyLayer, true); // 冲刺期间无敌

        // 消耗专注值
        s.character.currentFocus -= s.config.lightCutFocusCost;
        s.ctx.CurrentFocus = s.character.currentFocus;
        // s.character.OnHealthChange?.Invoke(s.character);
    }

    public void OnLightCutAnimFinished()
    {
        isAnimFinished = true;
        s.ctx.IsLightCutting = false;
    }

    public void LightCutMove(float cutForce)
    {
        s.motor.LightCutDash(s.ctx.FacingDirection, cutForce);
    }
}
