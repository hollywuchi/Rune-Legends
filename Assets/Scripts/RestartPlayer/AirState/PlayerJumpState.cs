using RestartPlayer.HFSM;
using UnityEngine;

public class PlayerJumpState : PlayerAirState
{
    public PlayerJumpState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        s.context.JumpCount++;

        // 跳跃起速：由motor负责
        s.motor.Jump(s.config.jumpForce);
    }

    public override Transition LogicUpdate()
    {
        // 先跑空中通用规则（可能请求 airSprint / doubleJump 等）
        var t = base.LogicUpdate();
        if (t.HasTarget) return t;

        // 上升结束 -> Fall
        if (s.motor.Velocity.y < 0 && !s.context.IsAttacking)
            return new Transition(PlayerStateId.Fall);

        return Transition.None;
    }

    public override void PhysicsUpdate()
    {
        float targetX = s.context.MoveInput.x * s.config.airMaxSpeed;
        float accel = Mathf.Abs(s.context.MoveInput.x) > 0.01f ? s.config.airAcceleration : s.config.airDeceleration;
        s.motor.MoveVelocityX(targetX, accel * Time.fixedDeltaTime);
    }
}