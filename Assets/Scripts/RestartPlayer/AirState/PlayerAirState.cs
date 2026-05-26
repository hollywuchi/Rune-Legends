using RestartPlayer.HFSM;
using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(PlayerServices s) : base(s) { }

    public override Transition LogicUpdate()
    {
        // 空中上劈攻击,这里使用地面检测来简单的区分上劈和空中上劈
        if (s.ctx.UpAttackPressedThisFrame && !s.ctx.IsAttacking)
        {
            // TODO:之后在这里写普通上劈
            // if (s.ctx.IsGrounded)
            //     return new Transition(PlayerStateId.UpAttack);
            // else
                return new Transition(PlayerStateId.AirUpAttack);
        }

        // 空中下劈攻击
        if (s.ctx.DownAttackPressedThisFrame && !s.ctx.IsAttacking)
        {
            return new Transition(PlayerStateId.AirDownAttack);
        }

        // 空中攻击（优先级最高）
        if (s.ctx.AttackPressedThisFrame && !s.ctx.IsAttacking)
        {
            return new Transition(PlayerStateId.AirAttack);
        }

        // 空中冲刺（域内规则）
        if (s.ctx.SprintPressedThisFrame && s.ctx.CanSprint)
            return new Transition(PlayerStateId.AirSprint);

        // 空中转向（安全：只做朝向，不切状态）
        if (s.ctx.MoveInput.x != 0 && Mathf.Sign(s.ctx.MoveInput.x) != s.ctx.FacingDirection && !s.ctx.IsTouchingWall)
        {
            s.ctx.FlipFacing();
            s.motor.FlipFacing();
        }

        // 二段跳
        if (s.ctx.JumpPressedThisFrame && s.ctx.JumpCount < s.ctx.MaxJumpCount && !s.ctx.IsTouchingWall)
        {
            return new Transition(PlayerStateId.Jump2);
        }

        return base.LogicUpdate();
    }
}