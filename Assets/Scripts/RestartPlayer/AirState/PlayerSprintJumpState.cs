using RestartPlayer.HFSM;
using UnityEngine;

public class PlayerSprintJumpState : PlayerAirState
{
    private int _jumpDir;
    private float _airTimer;

    public PlayerSprintJumpState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        s.context.JumpCount++;
        _jumpDir = s.context.FacingDirection;
        _airTimer = 0f;

        s.motor.SetVelocity(new Vector2(_jumpDir * s.config.sprintJumpSpeedX, s.config.sprintJumpForceY));
        s.anim.ResetCommonTriggers();
        // s.anim.CrossFadeToSJump(0);
        s.anim.ToSJumpBool(true);
    }

    public override Transition LogicUpdate()
    {
        if (s.context.IsDead)
            return new Transition(PlayerStateId.Death);

        if (s.context.IsHurt)
            return new Transition(PlayerStateId.Hurt);

        if (!s.context.IsAttacking && s.context.ConsumeUpAttack())
            return new Transition(PlayerStateId.AirUpAttack);

        if (!s.context.IsAttacking && s.context.ConsumeDownAttack())
            return new Transition(PlayerStateId.AirDownAttack);

        if (!s.context.IsAttacking && s.context.ConsumeAttack())
            return new Transition(PlayerStateId.AirAttack);

        if (s.context.CanSprint && s.context.ConsumeSprint())
            return new Transition(PlayerStateId.AirSprint);

        if (s.context.JumpCount < s.context.MaxJumpCount && !s.context.IsTouchingWall && s.context.ConsumeJump())
            return new Transition(PlayerStateId.Jump2);

        _airTimer += Time.deltaTime;

        if (_airTimer > 0.08f && s.motor.Velocity.y < 0f && ShouldWallSlide())
            return new Transition(PlayerStateId.WallSlide);

        if (_airTimer > 0.08f && s.context.IsGrounded && s.motor.Velocity.y <= 0.05f)
        {
            if (s.context.SprintIsHeld)
                return new Transition(PlayerStateId.SprintRun);

            return new Transition(PlayerStateId.Land);
        }

        return Transition.None;
    }

    public override void PhysicsUpdate()
    {
        s.motor.SetVelocityX(_jumpDir * s.config.sprintJumpSpeedX);
    }

    public override void Exit()
    {
        s.anim.ToSJumpBool(false);
    }

    private bool ShouldWallSlide()
    {
        if (s.context.IsTouchingLeftWall && s.context.IsTouchingRightWall)
            return false;
        if (s.context.IsTouchingLeftWall && s.context.FacingDirection == -1)
            return true;
        if (s.context.IsTouchingRightWall && s.context.FacingDirection == 1)
            return true;
        return false;
    }
}
