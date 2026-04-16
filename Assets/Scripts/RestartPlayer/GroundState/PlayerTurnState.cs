using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnState : PlayerGroundState
{
    public PlayerTurnState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        player.animator.Play("TrickTurn");
        Debug.Log("进入转身状态");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if(player.moveInput.x != 0 && Mathf.Sign(player.moveInput.x) == player.FacingDirection)
        {
            stateMachine.ChangeState(player.locomotionState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
