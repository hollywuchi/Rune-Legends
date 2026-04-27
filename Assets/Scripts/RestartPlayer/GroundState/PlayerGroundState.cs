using UnityEngine;
using RestartPlayer.HFSM;

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
        // 先做“域守卫”：离地 -> Fall
        if (!s.ctx.IsGrounded)
            return new Transition(PlayerStateId.Fall);

        // 地面输入：冲刺、跳
        if (s.ctx.SprintPressedThisFrame)
            return new Transition(PlayerStateId.Sprint);

        if (s.ctx.JumpPressedThisFrame && s.ctx.JumpBufferTime < 2)
            return new Transition(PlayerStateId.Jump);

        return base.LogicUpdate();
    }
}