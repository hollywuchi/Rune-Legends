using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class PlayerLocomotionState : PlayerGroundState
{
    public PlayerLocomotionState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("进入PlayerLocomotion状态");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Run();

        if (player.moveInput.x != 0 && Mathf.Sign(player.moveInput.x) != player.FacingDirection)
        {
            // 输入方向和面朝方向反了！立刻进入转身状态！
            stateMachine.ChangeState(player.turnState);
            return;
        }

        if (Mathf.Abs(player.moveInput.x) < 0.1f)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        // Run();
        base.PhysicsUpdate();
    }

    public override void Exit()
    {
        base.Exit();
        player.animator.SetFloat("InputX", 0);
    }


    private void Run()
    {
        player.rb.velocity = new Vector2(player.moveInput.x * player.Speed, player.rb.velocity.y);
    }
}
