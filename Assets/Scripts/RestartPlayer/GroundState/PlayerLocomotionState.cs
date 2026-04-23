using RestartPlayer.HFSM;
using UnityEngine;

public class PlayerLocomotionState : PlayerGroundState
{
    public PlayerLocomotionState(Player player, PlayerStateMachine stateMachine, PlayerContext ctx, PlayerAnimatorDriver anim, PlayerStateRegistry stateRegistry, PlayerMotor2D motor)
    : base(player, stateMachine, ctx, anim, stateRegistry, motor) { }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("进入PlayerLocomotion状态");
    }

    public override Transition LogicUpdate()
    {
        var t = base.LogicUpdate();
        if (t.HasTarget) return t;

        Run();

        if (ctx.MoveInput.x != 0 && Mathf.Sign(ctx.MoveInput.x) != ctx.FacingDirection)
            return new Transition(PlayerStateId.Turn);

        if (Mathf.Abs(ctx.MoveInput.x) < 0.1f)
            return new Transition(PlayerStateId.Idle);

        return Transition.None;
    }

    public override void Exit()
    {
        base.Exit();
        anim.SetInputX(0f);
    }

    private void Run()
    {
        motor.SetVelocityX(ctx.MoveInput.x * player.Speed);
    }
}