using RestartPlayer.HFSM;
using UnityEngine;

public class PlayerGroundState : PlayerState
{
    public PlayerGroundState(PlayerServices s) : base(s)
    {
    }

    public override void Enter()
    {
        base.Enter();
        s.context.JumpCount = 0;
        s.context.CanSprint = true;

        // 落地时土狼时间清零
        s.context.CoyoteTimer = 0f;
    }

    public override Transition LogicUpdate()
    {
        // 先调用基类的逻辑更新，处理受伤状态
        var t = base.LogicUpdate();
        if (t.HasTarget) return t;

        if (s.context.CanRest && s.context.RestPressedThisFrame)
            return new Transition(PlayerStateId.Rest);

        if (!s.context.IsGrounded)
            return new Transition(PlayerStateId.Fall);

        // 格挡输入检测：点按即可进入格挡状态
        if (s.context.IsGrounded && !s.context.IsPostureBroken && s.context.ConsumeBlock())
            return new Transition(PlayerStateId.Block);

        // 攻击输入检测
        if (!s.context.IsAttacking && s.context.ConsumeAttack())
            return new Transition(PlayerStateId.AttackCombo1);

        // 治愈技能输入检测
        if (s.context.SkillPressedThisFrame && !s.context.IsHealing && !s.context.IsAttacking
            && s.context.CurrentFocus >= s.config.maxFocus)
        {
            return new Transition(PlayerStateId.Heal);
        }

        // 霹雳一闪技能输入检测
        if (s.context.LightCutPressedThisFrame && !s.context.IsHealing &&
            s.context.CurrentFocus >= s.config.lightCutFocusCost)
        {
            return new Transition(PlayerStateId.LightCut);
        }

        // 攻击力Buff技能输入检测
        if (s.context.LightCrownPressedThisFrame && !s.context.IsBuffing && !s.context.IsAttacking
            && s.context.CurrentFocus >= s.config.LightCrownFocusCost
            && s.focusSystem.attackBuffStack < s.config.maxAttackBuffStack)
        {
            return new Transition(PlayerStateId.LightCrown);
        }

        // 地面输入：冲刺、跳
        if (s.context.ConsumeSprint())
            return new Transition(PlayerStateId.Sprint);

        if (s.context.ConsumeJump())
            return new Transition(PlayerStateId.Jump);

        return base.LogicUpdate();
    }
}