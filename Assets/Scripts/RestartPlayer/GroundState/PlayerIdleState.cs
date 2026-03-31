using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundState
{
    public PlayerIdleState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();  // base因为是继承的PlayerGroundState，地面检测都在其中
        if (Mathf.Abs(player.moveInput.x) > 0.01f)
            stateMachine.ChangeState(player.runState);
    }

    public override void Exit()
    {
        base.Exit();

    }
}
