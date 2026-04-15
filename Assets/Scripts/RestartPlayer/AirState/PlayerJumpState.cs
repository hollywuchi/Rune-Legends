using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerAirState
{
    public PlayerJumpState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("进入PlayerJump状态");
        // 在进入跳跃状态时，给角色一个向上的初始速度
        player.rb.AddForce(new Vector2(0, player.jumpForce), ForceMode2D.Impulse);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        player.animator.SetFloat("vecocityY", player.rb.velocity.y);

        if (player.inputActions.MoveSystem.Sprint.WasPressedThisFrame())
        {
            // 如果在空中按下冲刺键，进入冲刺状态
            // TODO:空中冲刺只能冲刺一次，这里没有做出限制
            stateMachine.ChangeState(player.sprintState);
            return;
        }
        if (player.physicsCheck.IsGround && Mathf.Abs(player.moveInput.x) < 0.1f)
        {
            player.animator.SetTrigger("Idle");
            stateMachine.ChangeState(player.idleState);
        }
        else if (player.physicsCheck.IsGround && Mathf.Abs(player.moveInput.x) >= 0.1f)
        {
            player.animator.SetTrigger("Ing");
            stateMachine.ChangeState(player.locomotionState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
