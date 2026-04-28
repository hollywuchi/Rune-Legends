using UnityEngine;
using RestartPlayer.HFSM;

public class PlayerWallJumpState : PlayerAirState
{
    public PlayerWallJumpState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        s.ctx.JumpCount++;

        // 换成直接添加力的方式，感觉更自然一些
        s.motor.WallJump(s.config.wallJumpForce, -s.ctx.FacingDirection, s.config.wallHorizontalBoost);
        s.inputGate.Freeze(0.1f);
        // TODO：可以添加特效

        Debug.Log("进入WallJump状态");

    }

    public override Transition LogicUpdate()
    {
        var t = base.LogicUpdate();
        if (t.HasTarget) return t;


        // 继承JumpState的空中规则（空中横移/Fall）
        if (!s.inputGate.IsFrozen)
            s.motor.SetVelocityX(s.ctx.MoveInput.x * s.config.speed * 0.5f);

        if (s.ctx.IsTouchingWall && s.ctx.MoveInput.x != 0)
            return new Transition(PlayerStateId.WallSlide);

        if (!s.ctx.IsTouchingWall && s.motor.Velocity.y < 0)
            return new Transition(PlayerStateId.Fall);
        return Transition.None;
    }
}
