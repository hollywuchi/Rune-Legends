using RestartPlayer.HFSM;
using UnityEngine;

public class PlayerSprintState : PlayerGroundState
{
    public PlayerSprintState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();

        s.ctx.IsSprintFinished = false;

        s.anim.ResetCommonTriggers();
        s.anim.PlayToSprint();

        s.fxSpeaker.CreateFX(s.motor.transform,s.ctx.FacingDirection,ParticalEffectType.SprintDust);
        s.motor.SetVelocityX(s.ctx.FacingDirection * s.config.sprintSpeed);

        Debug.Log("进入冲刺状态");
    }

    public override Transition LogicUpdate()
    {
        var t = base.LogicUpdate();
        if (t.HasTarget) return t;

        if (s.ctx.IsSprintFinished)
        {
            if (s.ctx.SprintIsHeld || Mathf.Abs(s.ctx.MoveInput.x) > 0.1f)
            {
                s.anim.TriggerIng();
                return new Transition(PlayerStateId.Locomotion);
            }
            s.anim.TriggerIdle();
            return new Transition(PlayerStateId.Idle);
        }

        return Transition.None;
    }
}