using RestartPlayer.HFSM;
using UnityEngine;

public class PlayerSprintRunState : PlayerGroundState
{
    public PlayerSprintRunState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        s.context.IsSprintRunning = true;

        if (Mathf.Abs(s.context.MoveInput.x) > 0.1f && Mathf.Sign(s.context.MoveInput.x) != s.context.FacingDirection)
        {
            s.context.FlipFacing();
            s.motor.FlipFacing();
        }

        s.anim.ResetCommonTriggers();
        s.anim.SetInputX(2f);
        s.anim.CrossFadeToIng(0.05f);
    }

    public override Transition LogicUpdate()
    {
        var t = base.LogicUpdate();
        if (t.HasTarget)
        {
            if (t.Target == PlayerStateId.Jump)
                return new Transition(PlayerStateId.SprintJump);

            if (t.Target != PlayerStateId.Sprint)
                return t;
        }

        if (!s.context.SprintIsHeld)
        {
            if (Mathf.Abs(s.context.MoveInput.x) > 0.1f)
                return new Transition(PlayerStateId.Locomotion);

            s.anim.TriggerIdle();
            return new Transition(PlayerStateId.Idle);
        }

        if ((s.context.IsTouchingLeftWall && s.context.FacingDirection == -1) ||
            (s.context.IsTouchingRightWall && s.context.FacingDirection == 1))
        {
            s.anim.TriggerIdle();
            return new Transition(PlayerStateId.Idle);
        }

        if (Mathf.Abs(s.context.MoveInput.x) > 0.1f && Mathf.Sign(s.context.MoveInput.x) != s.context.FacingDirection)
        {
            s.context.FlipFacing();
            s.motor.FlipFacing();
        }

        return Transition.None;
    }

    public override void PhysicsUpdate()
    {
        s.motor.SetVelocityX(s.context.FacingDirection * s.config.sprintRunSpeed);
    }

    public override void Exit()
    {
        base.Exit();
        s.context.IsSprintRunning = false;
    }
}
