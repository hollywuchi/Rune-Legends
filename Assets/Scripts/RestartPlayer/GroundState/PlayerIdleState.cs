using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundState
{
    public PlayerIdleState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        
        // WORKFLOW:这里写进入状态之后要做的事
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();  // base因为是继承的PlayerGroundState，地面检测都在其中
        // WORKFLOW:在这里添加状态切换条件
    }
}
