using RestartPlayer.HFSM;
using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(Player player, PlayerStateMachine stateMachine, PlayerContext ctx, PlayerAnimatorDriver anim, PlayerStateRegistry stateRegistry, PlayerMotor2D motor) 
    : base(player, stateMachine, ctx, anim, stateRegistry, motor) { }

    public override Transition LogicUpdate()
    {
        // 空中冲刺（域内规则）
        if (ctx.SprintPressedThisFrame && ctx.CanSprint)
            return new Transition(PlayerStateId.AirSprint);

        // 空中转向（安全：只做朝向，不切状态）
        if (ctx.MoveInput.x != 0 && Mathf.Sign(ctx.MoveInput.x) != ctx.FacingDirection)
            player.Flip();

        // 二段跳（注意：土狼时间在 FallState 里处理优先级，这一步先保持你逻辑）
        if (ctx.JumpPressedThisFrame && ctx.JumpCount < ctx.MaxJumpCount)
            return new Transition(PlayerStateId.Jump);

        return base.LogicUpdate();
    }
}