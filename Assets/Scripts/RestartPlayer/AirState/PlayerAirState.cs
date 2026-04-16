using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
    }


    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        // if(player.inputActions.MoveSystem.Sprint.WasPressedThisFrame())
        // {
        //     stateMachine.ChangeState(player.sprintState);
        // }   

        if (player.moveInput.x != 0 && Mathf.Sign(player.moveInput.x) != player.FacingDirection)
        {
            player.Flip();
        }
        
    }

    public override void Exit()
    {
        base.Exit();
    }
}
