using RestartPlayer.HFSM;
using UnityEngine;

public class PlayerLandState : PlayerGroundState
{
    private float _landTimer;

    public PlayerLandState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        _landTimer = 0f;
        s.anim.ResetCommonTriggers();
        s.anim.PlayBreakSJump();
    }

    public override Transition LogicUpdate()
    {
        _landTimer += Time.deltaTime;

        var t = base.LogicUpdate();
        if (t.HasTarget)
        {
            if (t.Target == PlayerStateId.Jump && s.context.SprintIsHeld)
                return new Transition(PlayerStateId.SprintJump);

            return t;
        }

        if (_landTimer >= s.config.sprintLandCancelTime && Mathf.Abs(s.context.MoveInput.x) > 0.1f)
        {
            if (s.context.SprintIsHeld)
                return new Transition(PlayerStateId.SprintRun);

            s.anim.TriggerIng();
            return new Transition(PlayerStateId.Locomotion);
        }

        if (_landTimer >= s.config.sprintLandDuration)
        {
            s.anim.TriggerIdle();
            return new Transition(PlayerStateId.Idle);
        }

        return Transition.None;
    }

    public override void PhysicsUpdate()
    {
        if (!s.motor.IsForceMoving)
            s.motor.MoveVelocityX(0f, s.config.landBrakeDeceleration * Time.fixedDeltaTime);
    }

    public override void Exit()
    {
        base.Exit();
        s.motor.StopForceMove();
    }
}
