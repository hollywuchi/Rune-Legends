using System.Collections;
using System.Collections.Generic;
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
        // 生成尘埃特效
        player.poolManager.CreateSprintDust(player.transform, player.FacingDirection, ParticalEffectType.SprintDust);
        // 给予一个初速度
        player.rb.velocity = new Vector2(player.FacingDirection * player.Speed, player.rb.velocity.y);
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
            else if(!player.physicsCheck.IsGround)
            {
                player.animator.SetTrigger("Fall");
                stateMachine.ChangeState(player.jumpState);
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
