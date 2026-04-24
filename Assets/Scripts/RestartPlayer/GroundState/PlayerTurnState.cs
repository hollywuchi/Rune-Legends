using RestartPlayer.HFSM;
using UnityEngine;

public class PlayerTurnState : PlayerGroundState
{
    public PlayerTurnState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        s.anim.PlayTrickTurn();
        Debug.Log("进入转身状态");
    }

    public override Transition LogicUpdate()
    {
        var t = base.LogicUpdate();
        if (t.HasTarget) return t;

        if (s.ctx.MoveInput.x != 0 && Mathf.Sign(s.ctx.MoveInput.x) == s.ctx.FacingDirection)
            return new Transition(PlayerStateId.Locomotion);

        return Transition.None;
    }
}