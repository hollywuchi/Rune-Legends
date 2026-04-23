using RestartPlayer.HFSM;

public class PlayerGroundState : PlayerState
{
    public PlayerGroundState(Player player, PlayerStateMachine stateMachine, PlayerContext ctx, PlayerAnimatorDriver anim, PlayerStateRegistry stateRegistry, PlayerMotor2D motor)
     : base(player, stateMachine, ctx, anim, stateRegistry, motor) { }

    public override void Enter()
    {
        base.Enter();
        ctx.JumpCount = 0;
        ctx.CanSprint = true;

        // 落地时土狼时间清零
        ctx.CoyoteTimer = 0f;
    }

    public override Transition LogicUpdate()
    {
        // 先做“域守卫”：离地 -> Fall
        if (!ctx.IsGrounded)
            return new Transition(PlayerStateId.Fall);

        // 地面输入：冲刺、跳
        if (ctx.SprintPressedThisFrame)
            return new Transition(PlayerStateId.Sprint);

        if (ctx.JumpPressedThisFrame)
            return new Transition(PlayerStateId.Jump);

        return base.LogicUpdate();
    }
}