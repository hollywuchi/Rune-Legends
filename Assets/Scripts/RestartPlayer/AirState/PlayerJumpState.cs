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
        // 在进入跳跃状态时，给角色一个向上的初始速度
        // player.rb.AddForce(new Vector2(0, player.jumpForce), ForceMode2D.Impulse);
        // 将角色的跳跃速度改为直接控制
        player.rb.velocity = new Vector2(0, player.jumpForce);

        if (Mathf.Abs(player.moveInput.x) > 1)
        {
            player.animator.CrossFade("ToSJump", 0.05f);
        }
        Debug.Log("进入PlayerJump状态");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        // 如果当前切换了状态，就不再执行下面的代码，避免在切换状态后还继续执行当前状态的逻辑
        if (stateMachine.currentState != this) return;
        player.rb.velocity = new Vector2(player.moveInput.x * player.Speed * 0.5f, player.rb.velocity.y);

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
