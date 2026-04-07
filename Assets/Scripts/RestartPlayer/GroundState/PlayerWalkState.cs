using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Unity.Mathematics;
using UnityEngine;

public class PlayerWalkState : PlayerGroundState
{
    public PlayerWalkState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        // player.animator.SetBool("Conversion", false);
        player.animator.SetBool("IsWalking", true);
        Debug.Log("进入Walk状态");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        // 走路应该和跑步是一个逻辑，为什么不继承RunState？因为这两个状态也需要相互转换
        if (player.moveInput.x != 0 && Mathf.Sign(player.moveInput.x) != player.FacingDirection)
        {
            // 输入方向和面朝方向反了！立刻进入转身状态！
            stateMachine.ChangeState(player.turnState);
            return;
        }

        // if (Mathf.Abs(player.moveInput.x) < 0.01f && Mathf.Abs(player.rb.velocity.x) < 0.1f)
        if (Mathf.Abs(player.moveInput.x) <= 0.1f)
        {
            stateMachine.ChangeState(player.idleState);
            // 当玩家不动的时候，return回去，防止其他操作
            return;
        }

        if (Mathf.Abs(player.moveInput.x) > 0.3f)
        {
            // 同样的，高于某个数值时，就要切换到跑步状态
            player.animator.SetTrigger("Conversion");
            stateMachine.ChangeState(player.runState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        Walk();
    }

    public override void Exit()
    {
        base.Exit();
        player.animator.SetBool("IsWalking", false);
    }

    private void Walk()
    {
        Vector2 dir = player.inputActions.MoveSystem.WalkOrRun.ReadValue<Vector2>();

        player.rb.velocity = new Vector2(dir.x * player.Speed * Time.deltaTime, player.rb.velocity.y);

    }

}
