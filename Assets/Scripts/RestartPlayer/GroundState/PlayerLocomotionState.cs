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
        base.PhysicsUpdate();
        Run();
    }

    public override void Exit()
    {
        base.Exit();
    }


    private void Run()
    {
        Vector2 dir = player.inputActions.MoveSystem.WalkOrRun.ReadValue<Vector2>();

        // TODO: 这里的速度设置有点问题，应该是根据输入的大小来调整速度的，而不是直接乘以一个固定的数值。
        // if (player.inputActions.MoveSystem.Sprint.IsPressed())
        // {
        //     player.Speed *= 2;
        // }

        player.rb.velocity = new Vector2(dir.x * player.Speed * Time.deltaTime, player.rb.velocity.y);

    }
}
