using RestartPlayer.HFSM;
using UnityEngine;

public class PlayerIdleState : PlayerGroundState
{
    public PlayerIdleState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
    }

    public override Transition LogicUpdate()
    {
        var t = base.LogicUpdate();
        if (t.HasTarget) return t;

        if (Mathf.Abs(s.context.MoveInput.x) > 0.1f)
        {
            if (s.context.SprintIsHeld)
                return new Transition(PlayerStateId.Sprint);

            return new Transition(PlayerStateId.Locomotion);
        }

        return Transition.None;
    }
}