using RestartPlayer.HFSM;
using UnityEngine;

public class PlayerFallState : PlayerAirState
{
    public PlayerFallState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();

        // 只有“刚离地且未发生跳跃”的情况下才给土狼时间
        // 这里用 JumpCount==0 替代你之前的 jumpTime==0
        s.context.CoyoteTimer = (s.context.JumpCount == 0) ? s.context.CoyoteTime : 0f;
    }

    public override Transition LogicUpdate()
    {
        // 1) 土狼时间优先级最高：在 base(Air) 处理二段跳前先截获
        if (s.context.CoyoteTimer > 0f)
        {
            s.context.CoyoteTimer -= Time.deltaTime;

            if (s.context.ConsumeJump())
            {
                s.context.CoyoteTimer = 0f;
                return new Transition(PlayerStateId.Jump);
            }
        }

        // 2) 空中通用规则（空冲/翻面/二段跳）
        var t = base.LogicUpdate();
        if (t.HasTarget) return t;

        // 3) 碰墙且有横向输入 -> WallSlide
        if (ShouldWallSlide())
            return new Transition(PlayerStateId.WallSlide);

        // 4) 空中横移
        s.motor.SetVelocityX(s.context.MoveInput.x * s.config.speed * s.context.AirSprintMag);

        // 5) 落地 -> Idle/Locomotion
        if (s.context.IsGrounded)
        {
            if (Mathf.Abs(s.context.MoveInput.x) < 0.1f)
            {
                s.anim.TriggerIdle();
                return new Transition(PlayerStateId.Idle);
            }
            else
            {
                s.anim.TriggerIng();
                return new Transition(PlayerStateId.Locomotion);
            }
        }

        return Transition.None;
    }

    /// <summary>
    /// 如果玩家碰到墙了，并且和墙一个朝向，那么就应该进入WallSlide状态
    /// </summary>
    /// <returns></returns>
    private bool ShouldWallSlide()
    {
        if(s.context.IsTouchingLeftWall && s.context.IsTouchingRightWall)
            return false;
        if(s.context.IsTouchingLeftWall && s.context.FacingDirection == -1)
            return true;
        if(s.context.IsTouchingRightWall && s.context.FacingDirection == 1)
            return true;
        return false;
    }
}