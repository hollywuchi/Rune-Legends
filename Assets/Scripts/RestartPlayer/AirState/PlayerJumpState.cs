using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerJumpState : PlayerAirState
{
    public PlayerJumpState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        if (player.jumpTime > 2) return;
        else player.jumpTime++;
        base.Enter();
        Debug.Log("进入PlayerJump状态");
        // 在进入跳跃状态时，给角色一个向上的初始速度
        // player.rb.AddForce(new Vector2(0, player.jumpForce), ForceMode2D.Impulse);
        // 将角色的跳跃速度改为直接控制
        player.rb.velocity = new Vector2(player.rb.velocity.x, player.jumpForce);

        if (Mathf.Abs(player.moveInput.x) > 1)
        {
            player.animator.CrossFade("ToSJump", 0.05f);
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (player.rb.velocity.y < 0)
        {
            stateMachine.ChangeState(player.fallState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
