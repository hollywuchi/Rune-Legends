using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerAirState
{
    private float coyoteTime = 0.2f;
    private float coyoteTimer;
    public PlayerFallState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }
    public override void Enter()
    {
        base.Enter();
        if (player.jumpTime == 0)
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer = 0;
        }
        Debug.Log("进入PlayerFall状态");
    }
    public override void LogicUpdate()
    {

        // 土狼时间
        if (coyoteTimer > 0)
        {
            coyoteTimer -= Time.deltaTime;

            if (player.inputActions.MoveSystem.Jump.WasPressedThisFrame())
            {
                coyoteTimer = 0;
                stateMachine.ChangeState(player.jumpState);
                Debug.Log("土狼时间！");
                return;
            }
        }
        // 土狼时间要比二段跳优先级高
        base.LogicUpdate();
        if (player.physicsCheck.IsGround)
        {
            if (Mathf.Abs(player.moveInput.x) < 0.1f)
            {
                player.animator.SetTrigger("Idle");
                Debug.Log("这是PlayerFall传来的Idle信号");
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
