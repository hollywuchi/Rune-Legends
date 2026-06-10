using RestartPlayer.HFSM;
using UnityEngine;

public class PlayerGroundState : PlayerState
{
    public PlayerGroundState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        s.ctx.JumpCount = 0;
        s.ctx.CanSprint = true;

        // 落地时土狼时间清零
        s.ctx.CoyoteTimer = 0f;
    }

    public override Transition LogicUpdate()
    {
        base.LogicUpdate(); // 先调用基类的逻辑更新，处理受伤状态

        if (s.ctx.CanRest && s.ctx.RestPressedThisFrame)
            return new Transition(PlayerStateId.Rest);

        // 先做"域守卫"：离地 -> Fall
        if (!s.ctx.IsGrounded)
            return new Transition(PlayerStateId.Fall);

        // 格挡输入检测：点按即可进入格挡状态
        if (s.ctx.BlockPressedThisFrame && s.ctx.IsGrounded && !s.ctx.IsPostureBroken)
            return new Transition(PlayerStateId.Block);

        // 攻击输入检测
        if (s.ctx.AttackPressedThisFrame && !s.ctx.IsAttacking)
            return new Transition(PlayerStateId.AttackCombo1);

        // 治愈技能输入检测
        if (s.ctx.SkillPressedThisFrame && !s.ctx.IsHealing && !s.ctx.IsAttacking
            && s.ctx.CurrentFocus >= s.config.maxFocus)
        {
            return new Transition(PlayerStateId.Heal);
        }

        // 霹雳一闪技能输入检测
        if (s.ctx.LightCutPressedThisFrame && !s.ctx.IsHealing && !s.ctx.IsAttacking
            && s.ctx.CurrentFocus >= s.config.lightCutFocusCost)
        {
            return new Transition(PlayerStateId.LightCut);
        }

        // 攻击力Buff技能输入检测
        if (s.ctx.LightCrownPressedThisFrame && !s.ctx.IsBuffing && !s.ctx.IsAttacking
            && s.ctx.CurrentFocus >= s.config.LightCrownFocusCost
            && s.character.attackBuffStack < s.config.maxAttackBuffStack)
        {
            return new Transition(PlayerStateId.LightCrown);
        }

        // 地面输入：冲刺、跳
        if (s.ctx.SprintPressedThisFrame)
            return new Transition(PlayerStateId.Sprint);

        if (s.ctx.JumpPressedThisFrame)
            return new Transition(PlayerStateId.Jump);

        return base.LogicUpdate();
    }


    // public override Transition LogicUpdate()
    // {
    //     // 先做“域守卫”：离地 -> Fall
    //     if (!s.ctx.IsGrounded)
    //         return new Transition(PlayerStateId.Fall);

    //     // 地面输入：冲刺、跳
    //     if (s.ctx.SprintPressedThisFrame)
    //         return new Transition(PlayerStateId.Sprint);

    //     if (s.ctx.JumpPressedThisFrame)
    //         return new Transition(PlayerStateId.Jump);

    //     return base.LogicUpdate();
    // }
}