using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerIdleState : PlayerGroundState
{
    public PlayerIdleState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        // player.animator.SetBool("Conversion", false);
        Debug.Log("进入Idle状态");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();  // base因为是继承的PlayerGroundState，地面检测都在其中
        if (Mathf.Abs(player.moveInput.x) > 0.3f)
            stateMachine.ChangeState(player.runState);
        else if (Mathf.Abs(player.moveInput.x) > 0.1f)
            stateMachine.ChangeState(player.walkState);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
