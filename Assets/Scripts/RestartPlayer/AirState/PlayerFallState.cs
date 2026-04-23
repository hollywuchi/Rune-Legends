using RestartPlayer.HFSM;
using UnityEngine;

public class PlayerFallState : PlayerAirState
{
    public PlayerFallState(Player player, PlayerStateMachine stateMachine, PlayerContext ctx, PlayerAnimatorDriver anim, PlayerStateRegistry registry, PlayerMotor2D motor)
    : base(player, stateMachine, ctx, anim, registry, motor) { }

    public override void Enter()
    {
        base.Enter();

        // 只有“刚离地且未发生跳跃”的情况下才给土狼时间
        // 这里用 JumpCount==0 替代你之前的 jumpTime==0
        ctx.CoyoteTimer = (ctx.JumpCount == 0) ? ctx.CoyoteTime : 0f;

        Debug.Log("进入PlayerFall状态");
    }

    public override Transition LogicUpdate()
    {
        // 1) 土狼时间优先级最高：在 base(Air) 处理二段跳前先截获
        if (ctx.CoyoteTimer > 0f)
        {
            ctx.CoyoteTimer -= Time.deltaTime;

            if (ctx.JumpPressedThisFrame)
            {
                ctx.CoyoteTimer = 0f;
                Debug.Log("土狼时间！");
                return new Transition(PlayerStateId.Jump);
            }
        }

        // 2) 空中通用规则（空冲/翻面/二段跳）
        var t = base.LogicUpdate();
        if (t.HasTarget) return t;

        // 3) 空中横移
        motor.SetVelocityX(ctx.MoveInput.x * player.Speed * 0.5f);

        // 4) 落地 -> Idle/Locomotion
        if (ctx.IsGrounded)
        {
            if (Mathf.Abs(ctx.MoveInput.x) < 0.1f)
                return new Transition(PlayerStateId.Idle);
            else
                return new Transition(PlayerStateId.Locomotion);
        }

        return Transition.None;
    }
}