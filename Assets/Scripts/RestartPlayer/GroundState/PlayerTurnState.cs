using RestartPlayer.HFSM;
using UnityEngine;

public class PlayerTurnState : PlayerGroundState
{
    public PlayerTurnState(Player player, PlayerStateMachine stateMachine, PlayerContext ctx, PlayerAnimatorDriver anim, PlayerStateRegistry stateRegistry, PlayerMotor2D motor) 
    : base(player, stateMachine, ctx, anim, stateRegistry, motor) { }

    public override void Enter()
    {
        base.Enter();
        anim.PlayTrickTurn();
        Debug.Log("进入转身状态");
    }

    public override Transition LogicUpdate()
    {
        var t = base.LogicUpdate();
        if (t.HasTarget) return t;

        if (ctx.MoveInput.x != 0 && Mathf.Sign(ctx.MoveInput.x) == ctx.FacingDirection)
            return new Transition(PlayerStateId.Locomotion);

        return Transition.None;
    }
}