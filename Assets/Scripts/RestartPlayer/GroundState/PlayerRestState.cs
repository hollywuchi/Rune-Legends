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
        s.character.currentFocus = s.config.maxFocus;
        s.ctx.CurrentFocus = s.config.maxFocus;
        s.character.OnHealthChange?.Invoke(s.character);
        s.character.resurrectPoint = s.ctx.ResurrectPoint;

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
        return Mathf.Abs(s.ctx.MoveInput.x) > 0.1f
            // || Mathf.Abs(s.ctx.MoveInput.y) > 0.1f
            || s.ctx.JumpPressedThisFrame
            || s.ctx.SprintPressedThisFrame
            || s.ctx.AttackPressedThisFrame
            || s.ctx.SkillPressedThisFrame
            || s.ctx.LightCutPressedThisFrame
            || s.ctx.LightCrownPressedThisFrame;
    }
}
