using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerAirState
{
    public PlayerFallState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }
    public override void Enter()
    {
        base.Enter();
        Debug.Log("进入PlayerFall状态");
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (player.physicsCheck.IsGround)
        {
            if (Mathf.Abs(player.moveInput.x) < 0.1f)
            {
                player.animator.SetTrigger("Idle");
                stateMachine.ChangeState(player.idleState);
            }
            else
            {
                player.animator.SetTrigger("Ing");
                stateMachine.ChangeState(player.locomotionState);
            }
        }
    }
     public override void Exit()
    {
        base.Exit();
    }
}
