using RestartPlayer.HFSM;
using UnityEngine;

public class PlayerRestState : PlayerState
{
    public PlayerRestState(PlayerServices s) : base(s) { }

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

    public override void Exit()
    {
        base.Exit();
    }
}
