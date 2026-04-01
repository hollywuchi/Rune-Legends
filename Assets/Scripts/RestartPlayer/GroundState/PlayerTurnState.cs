using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnState : PlayerGroundState
{
    public PlayerTurnState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        // player.animator.SetBool("IsTurn",true);
        player.animator.Play("TrickTurn");
        Debug.Log("进入转身状态");
    }

    public override void LogicUpdate()
    {
        // base.LogicUpdate();
        // AnimatorStateInfo info = player.animator.GetCurrentAnimatorStateInfo(0);
        // if (info.normalizedTime == 1)
        // {
        //     // player.animator.SetBool("IsTurn",false);
        //     player.ChackMoveState();
        // }

        if(player.moveInput.x != 0 && Mathf.Sign(player.moveInput.x) == player.FacingDirection)
        {
            stateMachine.ChangeState(player.runState);
        }


        if(isAnimationFinished)
        {
            player.Flip();

            if(player.moveInput.x == 0)
            {
                stateMachine.ChangeState(player.idleState);
            }
            else
            {
                stateMachine.ChangeState(player.runState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
