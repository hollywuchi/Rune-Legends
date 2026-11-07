using RestartPlayer.HFSM;
using UnityEngine;

public class PlayerRestState : PlayerGroundState
{
    private const float SleepDelaySeconds = 3f;

    private float restIdleTimer;

    public PlayerRestState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        s.character.CurrentHealth = s.character.maxHealth;
        // s.character.currentFocus = s.config.maxFocus;
        // s.context.CurrentFocus = s.config.maxFocus;
        // s.character.OnHealthChange?.Invoke(s.character);
        EventBus<FocusChangeEvent>.Raise(new FocusChangeEvent {isGetFous = true,changeFocusCount = 999});
        // EventBus<HealthChangeEvent>.Raise(new HealthChangeEvent{character = s.character});
        s.character.RestartHealth();
        
        //FIXME:这里更像是存档点，之后做存档的时候来做
        // s.character.resurrectPoint = s.context.ResurrectPoint;

        restIdleTimer = 0f;

        s.motor.SetVelocity(Vector2.zero);
        s.anim.SetIsResting();

    }

    public override Transition LogicUpdate()
    {
        if (HasAnyInput() && restIdleTimer > 1f)
        {
            return new Transition(PlayerStateId.Idle);
        }

        restIdleTimer += Time.deltaTime;
        if (restIdleTimer >= SleepDelaySeconds)
        {
            s.anim.SetIsSleeping();
        }

        return Transition.None;
    }

    public override void Exit()
    {
        restIdleTimer = 0f;
        s.anim.PlayBreakRest();
        s.inputGate.Freeze(s.anim.GetAnimationLength("BreakRest"));
        base.Exit();
    }

    private bool HasAnyInput()
    {
        return Mathf.Abs(s.context.MoveInput.x) > 0.1f
               // || Mathf.Abs(s.ctx.MoveInput.y) > 0.1f
               // || s.context.JumpPressedThisFrame
               // || s.context.SprintPressedThisFrame
               // || s.context.AttackPressedThisFrame
               // || s.context.SkillPressedThisFrame
               // || s.context.LightCutPressedThisFrame
               // || s.context.LightCrownPressedThisFrame;
               || s.context.ConsumeJump()
               || s.context.ConsumeSprint()
               || s.context.ConsumeAttack()
               || s.context.ConsumeBlock()
               || s.context.ConsumeDownAttack()
               || s.context.ConsumeUpAttack();
    }
}
