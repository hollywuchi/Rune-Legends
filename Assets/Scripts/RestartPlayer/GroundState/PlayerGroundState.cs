using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundState : PlayerState
{
    public PlayerGroundState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }
    public override void Enter()
    {
        base.Enter();
        player.jumpTime = 0;
        player.canSprint = true;
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (!player.isGround)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }

        if (player.isGround)
        {
            if (player.inputActions.MoveSystem.Sprint.WasPressedThisFrame())
            {
                stateMachine.ChangeState(player.sprintState);
                return;
            }

            if (player.inputActions.MoveSystem.Jump.WasPressedThisFrame())
            {
                stateMachine.ChangeState(player.jumpState);
            }
        }

    }

}
