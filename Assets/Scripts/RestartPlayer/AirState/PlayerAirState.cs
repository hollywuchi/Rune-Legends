using RestartPlayer.HFSM;
using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(PlayerServices s) : base(s) { }

    public override Transition LogicUpdate()
    {
        base.LogicUpdate(); // 先调用基类的逻辑更新，处理受伤状态
        // 受伤状态检查（优先级最高）
        // if (s.ctx.IsHurt)
        //     return Transition.None;

        // 空中上劈攻击,这里使用地面检测来简单的区分上劈和空中上劈
        if (!s.context.IsAttacking && s.context.ConsumeUpAttack())
        {
            return new Transition(PlayerStateId.AirUpAttack);
        }

        // 空中下劈攻击
        if (!s.context.IsAttacking && s.context.ConsumeDownAttack())
        {
            return new Transition(PlayerStateId.AirDownAttack);
        }

        // 空中攻击（优先级最高）
        if (!s.context.IsAttacking && s.context.ConsumeAttack())
        {
            return new Transition(PlayerStateId.AirAttack);
        }

        // 空中冲刺（域内规则）
        if ( s.context.CanSprint && s.context.ConsumeSprint())
            return new Transition(PlayerStateId.AirSprint);

        // 空中转向（安全：只做朝向，不切状态）
        if (!Mathf.Approximately(s.context.MoveInput.x,0) && Mathf.Sign(s.context.MoveInput.x) != s.context.FacingDirection && !s.context.IsTouchingWall)
        {
            s.context.FlipFacing();
            s.motor.FlipFacing();
        }

        // 二段跳
        if (s.context.JumpCount < s.context.MaxJumpCount && !s.context.IsTouchingWall && s.context.ConsumeJump())
        {
            return new Transition(PlayerStateId.Jump2);
        }

        return base.LogicUpdate();
    }
}