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
            // TODO:切换跳跃状态,但是跳跃状态都是子状态，至于为什么要切换还不是很懂
        }

    }
        
}
