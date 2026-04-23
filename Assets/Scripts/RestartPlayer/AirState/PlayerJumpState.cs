using RestartPlayer.HFSM;
using UnityEngine;

public class PlayerJumpState : PlayerAirState
{
    public PlayerJumpState(Player player, PlayerStateMachine stateMachine, PlayerContext ctx, PlayerAnimatorDriver anim, PlayerStateRegistry stateRegistry, PlayerMotor2D motor) 
    : base(player, stateMachine, ctx, anim, stateRegistry, motor) { }

    public override void Enter()
    {
        // 计数（不再用 jumpTime float）
        if (ctx.JumpCount >= ctx.MaxJumpCount) return;
        ctx.JumpCount++;

        base.Enter();

        // 跳跃起速：由motor负责
        motor.Jump(player.jumpForce);

        if (Mathf.Abs(ctx.MoveInput.x) > 1f)
            anim.CrossFadeToSJump(0.05f);

        Debug.Log("进入PlayerJump状态");
    }

    public override Transition LogicUpdate()
    {
        // 先跑空中通用规则（可能请求 airSprint / doubleJump 等）
        var t = base.LogicUpdate();
        if (t.HasTarget) return t;

        // 空中横移（叶子状态做细节：你原来是0.5倍率）
        motor.SetVelocityX(ctx.MoveInput.x * player.Speed * 0.5f);

        // 上升结束 -> Fall
        if (motor.Velocity.y < 0)
            return new Transition(PlayerStateId.Fall);

        return Transition.None;
    }
}