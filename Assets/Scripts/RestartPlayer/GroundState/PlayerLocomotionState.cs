using System;
using RestartPlayer.HFSM;

using UnityEngine;

public class PlayerLocomotionState : PlayerGroundState
{
    public PlayerLocomotionState(PlayerServices s) : base(s) { }

    private float _TurnTime;
    public override void Enter()
    {
        base.Enter();
        _TurnTime = s.context.TurnTime;
        // s.anim.ReSetBresk();
    }

    public override Transition LogicUpdate()
    {
        var t = base.LogicUpdate();
        if (t.HasTarget) return t;

        Run();

        if (MathF.Abs(s.context.MoveInput.x) >= 0.1f)
        {
            _TurnTime = s.context.TurnTime;

            if (Mathf.Sign(s.context.MoveInput.x) != s.context.FacingDirection)
            {
                s.context.FlipFacing();
                s.motor.FlipFacing();
            }

            return Transition.None;
        }

        _TurnTime -= Time.deltaTime;

        if (_TurnTime <= 0f)
        {
            // s.anim.SetBreak();
            return new Transition(PlayerStateId.Idle);
        }

        return Transition.None;
    }

    public override void Exit()
    {
        base.Exit();
        s.anim.SetInputX(0f);
    }

    private void Run()
    {
        s.motor.SetVelocityX(s.context.MoveInput.x * s.config.speed);
    }
}