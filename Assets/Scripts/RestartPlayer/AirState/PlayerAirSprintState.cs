using RestartPlayer.HFSM;
using UnityEngine;

public class PlayerAirSprintState : PlayerAirState
{
    public PlayerAirSprintState(Player player, PlayerStateMachine stateMachine, PlayerContext ctx, PlayerAnimatorDriver anim, PlayerStateRegistry stateRegistry, PlayerMotor2D motor) 
    : base(player, stateMachine, ctx, anim, stateRegistry, motor) { }

    public override void Enter()
    {
        base.Enter();

        anim.ResetCommonTriggers();
        anim.PlayToSprint();

        ctx.CanSprint = false;
        ctx.IsSprintFinished = false;

        motor.CaptureOriginalGravity();
        motor.GravityScale = 0f;

        player.poolManager.CreateFX(player.transform, ctx.FacingDirection, ParticalEffectType.AirDust);
        motor.DashHorizontal(ctx.FacingDirection, player.SprintSpeed);

        Debug.Log("进入空中冲刺状态");
    }

    public override Transition LogicUpdate()
    {
        // 空中冲刺期间：允许按跳打断（你的原逻辑）
        if (ctx.JumpPressedThisFrame)
            return new Transition(PlayerStateId.Jump);

        if (ctx.IsSprintFinished)
            return new Transition(PlayerStateId.Fall);

        return Transition.None;
    }

    public override void Exit()
    {
        base.Exit();
        player.motor.RestoreOriginalGravity();
    }
}