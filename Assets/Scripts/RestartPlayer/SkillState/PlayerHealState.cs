using RestartPlayer.HFSM;
using UnityEngine;

/// <summary>
/// 治愈技能状态
/// 只能在地面施法，消耗全部Focus值恢复1点血量
/// 被攻击时会被打断
/// </summary>
public class PlayerHealState : PlayerState
{
    private bool isAnimFinished;

    public PlayerHealState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        isAnimFinished = false;

        // 检查Focus是否足够
        if (s.context.CurrentFocus < s.config.maxFocus) return;

        // 设置治愈状态
        s.context.IsHealing = true;
        s.anim.SetIsHealing(true);

        // 停止移动
        s.motor.SetVelocityX(0);

        Debug.Log("进入治愈状态");
    }

    public override Transition LogicUpdate()
    {
        // 受伤状态检查（优先级最高）
        if (s.context.IsHurt)
        {
            // 受伤打断治愈，返回受伤状态
            return new Transition(PlayerStateId.Hurt);
        }

        // 动画完成，恢复血量
        if (s.context.SkillPerformedThisFrame)
        {
            HealPlayer();
            if (isAnimFinished)
                return new Transition(PlayerStateId.Idle);
        }
        else if (!s.context.IsHoldingSkill)
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
        // 治愈时保持静止
        s.motor.SetVelocityX(0f);
    }

    public override void Exit()
    {
        base.Exit();
        s.context.IsHealing = false;
        s.anim.SetIsHealing(false);
    }

    /// <summary>
    /// 恢复血量
    /// </summary>
    private void HealPlayer()
    {
        if (s.character != null)
        {
            EventBus<FocusChangeEvent>.Raise(new FocusChangeEvent { isGetFous = false,changeFocusCount = 999});
            s.character.HealthChange(s.config.healAmount);
        }
    }

    /// <summary>
    /// 动画事件回调：治愈动画完成
    /// </summary>
    public void OnHealAnimFinished()
    {
        isAnimFinished = true;
    }
}