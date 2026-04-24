using RestartPlayer.HFSM;
using UnityEngine;

public class PlayerLocomotionState : PlayerGroundState
{
    public PlayerLocomotionState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("进入PlayerLocomotion状态");
    }

    public override Transition LogicUpdate()
    {
        var t = base.LogicUpdate();
        if (t.HasTarget) return t;

        Run();

        if (s.ctx.MoveInput.x != 0 && Mathf.Sign(s.ctx.MoveInput.x) != s.ctx.FacingDirection)
            return new Transition(PlayerStateId.Turn);

        if (Mathf.Abs(s.ctx.MoveInput.x) < 0.1f)
            return new Transition(PlayerStateId.Idle);

        return Transition.None;
    }

    public override void Exit()
    {
        base.Exit();
        s.anim.SetInputX(0f);
    }

    private void Run()
    {
        s.motor.SetVelocityX(s.ctx.MoveInput.x * s.config.speed);
    }
}