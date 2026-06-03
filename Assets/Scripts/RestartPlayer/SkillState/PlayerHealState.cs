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
        if (s.ctx.CurrentFocus < s.config.maxFocus) return;

        // 设置治愈状态
        s.ctx.IsHealing = true;
        s.anim.SetIsHealing(true);

        // 停止移动
        s.motor.SetVelocityX(0);
        s.inputGate.Freeze(0.2f);

        Debug.Log("进入治愈状态");
    }

    public override Transition LogicUpdate()
    {
        // 受伤状态检查（优先级最高）
        if (s.ctx.IsHurt)
        {
            // 受伤打断治愈，返回受伤状态
            return new Transition(PlayerStateId.Hurt);
        }

        // 动画完成，恢复血量
        if (s.ctx.SkillPerformedThisFrame)
        {
            HealPlayer();
            Debug.Log("治愈完成，恢复血量");
            if (isAnimFinished)
                return new Transition(PlayerStateId.Idle);
        }
        else if (!s.ctx.IsHoldingSkill)
        {
            Debug.Log("治疗被取消");
            return new Transition(PlayerStateId.Idle);
        }

        // REVIEW:被攻击打断
        // if (!s.ctx.IsHealing)
        // {
        //     return new Transition(PlayerStateId.Idle);
        // }

        // 离地打断
        if (!s.ctx.IsGrounded)
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
        s.ctx.IsHealing = false;
        s.anim.SetIsHealing(false);
    }

    /// <summary>
    /// 恢复血量
    /// </summary>
    private void HealPlayer()
    {
        if (s.character != null)
        {
            s.character.CurrentHealth = Mathf.Min(
                s.character.CurrentHealth + s.config.healAmount,
                s.character.maxHealth
            );
            // 消耗全部Focus
            s.ctx.CurrentFocus = 0;
            s.character.currentFocus = 0;
            s.character.OnHealthChange?.Invoke(s.character);
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