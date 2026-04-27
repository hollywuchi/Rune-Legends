using RestartPlayer.HFSM;
using UnityEngine;

public class PlayerJump2State : PlayerJumpState
{
    public PlayerJump2State(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
    }

    public override Transition LogicUpdate()
    {
        var t = base.LogicUpdate();
        if (t.HasTarget) return t;

        return Transition.None;
    }
}