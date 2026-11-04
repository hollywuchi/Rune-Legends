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

        s.context.CanSprint = false;
        s.context.IsSprintFinished = false;

        s.motor.CaptureOriginalGravity();
        s.motor.GravityScale = 0f;

        // s.fxSpeaker.CreateFX(s.motor.transform.position,s.ctx.FacingDirection,ParticalEffectType.AirDust);
        EventBus<FxEvent>.Raise(new FxEvent{
            playerTran = s.motor.transform.position,
            dir = s.context.FacingDirection,
            type = ParticalEffectType.AirDust
        });
        s.motor.DashHorizontal(s.context.FacingDirection, s.config.sprintSpeed);
    }

    public override Transition LogicUpdate()
    {
        // 空中冲刺期间：允许按跳打断（你的原逻辑）
        // if (s.context.JumpPressedThisFrame)
        if(s.context.ConsumeJump())
            return new Transition(PlayerStateId.Jump);

        if (s.context.IsSprintFinished)
            return new Transition(PlayerStateId.Fall);

        return Transition.None;
    }

    public override void Exit()
    {
        base.Exit();
        s.motor.RestoreOriginalGravity();
    }
}