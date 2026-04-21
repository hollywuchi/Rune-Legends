using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirSprintState : PlayerAirState
{
    public PlayerAirSprintState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    private float originalGravity;

    public override void Enter()
    {
        base.Enter();
        player.animator.ResetTrigger("Idle");
        player.animator.ResetTrigger("Ing");
        player.animator.Play("ToSprint");
        player.canSprint = false;
        player.isSprintFinished = false;
        originalGravity = player.rb.gravityScale;
        player.rb.gravityScale = 0;
        // 生成尘埃特效
        player.poolManager.CreateFX(player.transform, player.FacingDirection, ParticalEffectType.AirDust);
        // 给予一个初速度
        player.rb.velocity = new Vector2(player.FacingDirection * player.SprintSpeed, 0);
        Debug.Log("进入空中冲刺状态");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (player.inputActions.MoveSystem.Jump.WasPressedThisFrame())
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }
        if (player.isSprintFinished)
        {
            player.stateMachine.ChangeState(player.fallState);
        }

    }

    public override void Exit()
    {
        base.Exit();
        player.rb.gravityScale = originalGravity;
    }
}
