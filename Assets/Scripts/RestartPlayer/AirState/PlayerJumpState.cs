using RestartPlayer.HFSM;
using UnityEngine;

public class PlayerJumpState : PlayerAirState
{
    public PlayerJumpState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        s.ctx.JumpCount++;

        // 跳跃起速：由motor负责
        s.motor.Jump(s.config.jumpForce);

        if (Mathf.Abs(s.ctx.MoveInput.x) > 1f)
            s.anim.CrossFadeToSJump(0.05f);

    }

    public override Transition LogicUpdate()
    {
        // 先跑空中通用规则（可能请求 airSprint / doubleJump 等）
        var t = base.LogicUpdate();
        if (t.HasTarget) return t;

        // 空中横移（叶子状态做细节：你原来是0.5倍率）
        s.motor.SetVelocityX(s.ctx.MoveInput.x * s.config.speed * 0.5f);

        // 上升结束 -> Fall
        if (s.motor.Velocity.y < 0)
            return new Transition(PlayerStateId.Fall);

        return Transition.None;
    }
}