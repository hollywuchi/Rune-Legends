using RestartPlayer.HFSM;
using UnityEngine;

// REVIEW:玩家休息状态
public class PlayerRestState : PlayerGroundState
{
    private const float SleepDelaySeconds = 3f;

    private enum RestPhase
    {
        EnteringRest,
        Resting,
        EnteringSleep,
        Sleeping,
        Breaking
    }

    private RestPhase currentPhase;
    private float restIdleTimer;

    public PlayerRestState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();

        s.character.CurrentHealth = s.character.maxHealth;
        s.character.currentFocus = s.config.maxFocus;
        s.ctx.CurrentFocus = s.config.maxFocus;
        s.character.OnHealthChange?.Invoke(s.character);

        restIdleTimer = 0f;
        currentPhase = RestPhase.EnteringRest;

        s.motor.SetVelocity(Vector2.zero);
        s.anim.PlayToRest();
    }

    public override Transition LogicUpdate()
    {
        if (s.ctx.IsHurt)
            return new Transition(PlayerStateId.Hurt);

        if (!s.ctx.IsGrounded)
            return new Transition(PlayerStateId.Fall);

        s.motor.SetVelocityX(0f);

        if (currentPhase == RestPhase.Breaking)
        {
            if (!s.anim.IsPlayingState("BreakRest", 3) || s.anim.IsStateFinished("BreakRest", 3))
                return new Transition(PlayerStateId.Idle);

            return Transition.None;
        }

        if (HasAnyInput())
        {
            currentPhase = RestPhase.Breaking;
            s.anim.PlayBreakRest();
            return Transition.None;
        }

        switch (currentPhase)
        {
            case RestPhase.EnteringRest:
                if (s.anim.IsPlayingState("Resting", 3) || s.anim.IsStateFinished("ToRest", 3))
                {
                    currentPhase = RestPhase.Resting;
                }
                break;

            case RestPhase.Resting:
                restIdleTimer += Time.deltaTime;
                if (restIdleTimer >= SleepDelaySeconds)
                {
                    currentPhase = RestPhase.EnteringSleep;
                    s.anim.PlayToSleep();
                }
                break;

            case RestPhase.EnteringSleep:
                if (s.anim.IsPlayingState("Sleeping", 3) || s.anim.IsStateFinished("ToSleep", 3))
                {
                    currentPhase = RestPhase.Sleeping;
                }
                break;

            case RestPhase.Sleeping:
                break;
        }

        return Transition.None;
    }

    public override void Exit()
    {
        restIdleTimer = 0f;
        currentPhase = RestPhase.EnteringRest;
        base.Exit();
    }

    private bool HasAnyInput()
    {
        return Mathf.Abs(s.ctx.MoveInput.x) > 0.1f
            || Mathf.Abs(s.ctx.MoveInput.y) > 0.1f
            || s.ctx.JumpPressedThisFrame
            || s.ctx.SprintPressedThisFrame
            || s.ctx.AttackPressedThisFrame
            || s.ctx.UpAttackPressedThisFrame
            || s.ctx.DownAttackPressedThisFrame
            || s.ctx.SkillPressedThisFrame
            || s.ctx.LightCutPressedThisFrame
            || s.ctx.LightCrownPressedThisFrame
            || s.ctx.ActivatePressedThisFrame
            || s.ctx.RestPressedThisFrame;
    }
}
