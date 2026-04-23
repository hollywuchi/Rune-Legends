using RestartPlayer.HFSM;
using UnityEngine;

public class PlayerSprintState : PlayerGroundState
{
    public PlayerSprintState(Player player, PlayerStateMachine stateMachine, PlayerContext ctx, PlayerAnimatorDriver anim, PlayerStateRegistry stateRegistry, PlayerMotor2D motor) 
    : base(player, stateMachine, ctx, anim, stateRegistry, motor) { }

    public override void Enter()
    {
        base.Enter();

        ctx.IsSprintFinished = false;

        anim.ResetCommonTriggers();
        anim.PlayToSprint();

        player.poolManager.CreateFX(player.transform, ctx.FacingDirection, ParticalEffectType.SprintDust);

        motor.SetVelocityX(ctx.FacingDirection * player.SprintSpeed);

        Debug.Log("进入冲刺状态");
    }

    public override Transition LogicUpdate()
    {
        var t = base.LogicUpdate();
        if (t.HasTarget) return t;

        if (ctx.IsSprintFinished)
        {
            if (ctx.SprintIsHeld || Mathf.Abs(ctx.MoveInput.x) > 0.1f)
                return new Transition(PlayerStateId.Locomotion);

            return new Transition(PlayerStateId.Idle);
        }

        return Transition.None;
    }
}