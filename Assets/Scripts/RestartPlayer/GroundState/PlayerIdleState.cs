using RestartPlayer.HFSM;
using UnityEngine;

public class PlayerIdleState : PlayerGroundState
{
    public PlayerIdleState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("进入Idle状态");
    }

    public override Transition LogicUpdate()
    {
        var t = base.LogicUpdate();
        if (t.HasTarget) return t;

        if (Mathf.Abs(s.ctx.MoveInput.x) > 0.1f)
            return new Transition(PlayerStateId.Locomotion);

        return Transition.None;
    }
}