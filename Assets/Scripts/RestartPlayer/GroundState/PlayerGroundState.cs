using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundState : PlayerState
{
    public PlayerGroundState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(player.inputActions.MoveSystem.Jump.IsPressed())
        {
            // stateMachine.ChangeState(player.jumpState)
        }

        // if(player.inputActions.MoveSystem.)
    }
        
}
