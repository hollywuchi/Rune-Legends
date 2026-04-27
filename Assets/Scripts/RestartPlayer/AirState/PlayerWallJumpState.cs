using UnityEngine;
using RestartPlayer.HFSM;

public class PlayerWallJumpState : PlayerJumpState
{
    public PlayerWallJumpState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        s.ctx.JumpCount++;

        s.motor.WallJump(s.config.wallHorizontalBoost, -s.ctx.FacingDirection, s.config.jumpForce);
        // TODO：可以添加特效

        Debug.Log("进入walljump状态");
    }

    public override Transition LogicUpdate()
    {
        var t = base.LogicUpdate();
        if (t.HasTarget) return t;

        // 继承JumpState的空中规则（空中横移/Fall）
        if (s.ctx.IsTouchingWall && s.ctx.MoveInput.x != 0)
            return new Transition(PlayerStateId.WallSlide);

        return Transition.None;
    }
}
