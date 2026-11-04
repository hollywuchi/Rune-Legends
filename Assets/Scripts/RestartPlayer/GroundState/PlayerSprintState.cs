using RestartPlayer.HFSM;
using UnityEngine;

public class PlayerSprintState : PlayerGroundState
{
    public PlayerSprintState(PlayerServices s) : base(s)
    {
    }

    public override void Enter()
    {
        base.Enter();

        s.context.IsSprintFinished = false;

        s.anim.ResetCommonTriggers();
        s.anim.PlayToSprint();

        if (s.context.MoveInput.x != 0)
        {
            if(s.context.FacingDirection != Mathf.Sign(s.context.MoveInput.x))
            {
                s.motor.FlipFacing();
                s.context.FlipFacing();
            }
            s.motor.SetVelocityX(Mathf.Sign(s.context.MoveInput.x) * s.config.sprintSpeed);
        }
        else
            s.motor.SetVelocityX(s.context.FacingDirection * s.config.sprintSpeed);
        
        EventBus<FxEvent>.Raise(new FxEvent
        {
            playerTran = s.motor.transform.position,
            dir = s.context.FacingDirection,
            type = ParticalEffectType.SprintDust
        });
    }

    public override Transition LogicUpdate()
    {
        var t = base.LogicUpdate();
        if (t.HasTarget) return t;

        if (s.context.IsSprintFinished)
        {
            if (s.context.SprintIsHeld || Mathf.Abs(s.context.MoveInput.x) > 0.1f)
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