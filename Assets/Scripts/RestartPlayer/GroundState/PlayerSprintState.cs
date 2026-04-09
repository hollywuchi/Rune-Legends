using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;

public class PlayerSprintState : PlayerState
{
    public bool isSprintFinished;
    public PlayerSprintState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        isSprintFinished = false;
        player.animator.Play("ToSprint");
        player.rb.velocity = new Vector2(player.FacingDirection * player.SprintSpeed, player.rb.velocity.y);
        Debug.Log("进入冲刺状态");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        // 如果当前冲刺动画已经播放完，玩家是否长按或者继续向前移动，如果是则进入移动状态，否则引入idle状态
        if (isSprintFinished)
        {
            if (player.inputActions.MoveSystem.Sprint.IsPressed() || Mathf.Abs(player.moveInput.x) > 0.1f)
            {
                player.animator.SetTrigger("Ing");
                stateMachine.ChangeState(player.locomotionState);
            }
            else
            {
                player.animator.SetTrigger("Idle");
                stateMachine.ChangeState(player.idleState);
            }
        }

    }
    public override void Exit()
    {
        base.Exit();
    }
}
