using RestartPlayer.HFSM;
using UnityEngine;

public class PlayerAirSprintState : PlayerAirState
{
    public PlayerAirSprintState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();

        s.anim.ResetCommonTriggers();
        s.anim.PlayToSprint();

        s.ctx.CanSprint = false;
        s.ctx.IsSprintFinished = false;

        s.motor.CaptureOriginalGravity();
        s.motor.GravityScale = 0f;

        s.fxSpeaker.CreateFX(s.motor.transform,s.ctx.FacingDirection,ParticalEffectType.AirDust);
        s.motor.DashHorizontal(s.ctx.FacingDirection, s.config.sprintSpeed);

        Debug.Log("进入空中冲刺状态");
    }

    public override Transition LogicUpdate()
    {
        // 空中冲刺期间：允许按跳打断（你的原逻辑）
        if (s.ctx.JumpPressedThisFrame)
            return new Transition(PlayerStateId.Jump);

        if (s.ctx.IsSprintFinished)
            return new Transition(PlayerStateId.Fall);

        return Transition.None;
    }

    public override void Exit()
    {
        base.Exit();
        s.motor.RestoreOriginalGravity();
    }
}