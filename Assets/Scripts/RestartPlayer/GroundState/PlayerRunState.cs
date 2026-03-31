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

        player.ChackMoveState();

        // 当玩家输入方向和速度方向不匹配的时候，就认为是转向了
        if (player.moveInput.x * player.rb.velocity.x <= 0)
        {
            stateMachine.ChangeState(player.turnState);
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
    }

    private void Run()
    {
        // player.inputActions.MoveSystem.WalkOrRun
        Vector2 dir = player.inputActions.MoveSystem.WalkOrRun.ReadValue<Vector2>();

        player.rb.velocity = new Vector2(dir.x * player.Speed * Time.deltaTime, player.rb.velocity.y);
    }

}