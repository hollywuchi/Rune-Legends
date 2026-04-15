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

        if(player.physicsCheck.IsGround)
        {
            // 这里要添加更高级的判定,比如说土狼时间之类的
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
