using System.Collections;
using System.Collections.Generic;
using RestartPlayer.HFSM;
using UnityEngine;

public class PlayerWallSlideState : PlayerAirState
{
    public PlayerWallSlideState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        s.motor.FlipFacing();
        s.ctx.JumpCount = 0; // 重置跳跃计数，允许玩家在墙上进行多段跳跃
        Debug.Log("进入WallSlide状态");
    }

    public override Transition LogicUpdate()
    {
        var t = base.LogicUpdate();
        if (t.HasTarget) return t;

        WallSlide();

        if (s.ctx.IsTouchingWall && !s.ctx.IsTouchingTopWall)
            return new Transition(PlayerStateId.Climb);
        if (s.ctx.JumpPressedThisFrame)
            return new Transition(PlayerStateId.WallJump);
        if (s.ctx.IsGrounded)
            return new Transition(PlayerStateId.Idle);
        if (!s.ctx.IsTouchingWall)
            return new Transition(PlayerStateId.Fall);

        return Transition.None;
    }
    public override void Exit()
    {
        base.Exit();
        s.motor.FlipFacing();
        Debug.Log("退出WallSlide状态");
    }

    private void WallSlide() => s.motor.SetVelocityY(-s.config.wallSlideSpeed);

}
