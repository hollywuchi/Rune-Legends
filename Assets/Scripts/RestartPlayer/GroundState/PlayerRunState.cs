using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerRunState : PlayerGroundState
{
    public PlayerRunState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        player.animator.SetBool("IsRunning", true);
        Debug.Log("进入跑步状态");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (player.moveInput.x != 0 && Mathf.Sign(player.moveInput.x) != player.FacingDirection)
        {
            // 输入方向和面朝方向反了！立刻进入转身状态！
            stateMachine.ChangeState(player.turnState);
            return; 
        }
        
        if (Mathf.Abs(player.moveInput.x) < 0.01f && Mathf.Abs(player.rb.velocity.x) < 0.1f)
            {
                stateMachine.ChangeState(player.idleState);
                // 当玩家不动的时候，return回去，防止其他操作
                return;
            }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        Run();
    }

    public override void Exit()
    {
        base.Exit();
        player.animator.SetBool("IsRunning", false);
        Debug.Log("退出Run状态");
    }

    private void Run()
    {
        Vector2 dir = player.inputActions.MoveSystem.WalkOrRun.ReadValue<Vector2>();

        player.rb.velocity = new Vector2(dir.x * player.Speed * Time.deltaTime, player.rb.velocity.y);

        // Debug.Log(player.rb.velocity);
    }

}