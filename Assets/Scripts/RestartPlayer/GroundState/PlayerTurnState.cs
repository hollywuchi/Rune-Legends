using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnState : PlayerGroundState
{
    public PlayerTurnState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        player.animator.SetTrigger("IsTurn");
        Debug.Log("进入转身状态");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        AnimatorStateInfo info = player.animator.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime == 1)
        {
            player.ChackMoveState();
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.CheckTurn();
    }
}
