using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
    }


    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (Mathf.Abs(player.moveInput.x) <= 1)
            player.rb.velocity = new Vector2(player.moveInput.x * player.Speed * 0.5f, player.rb.velocity.y);

        if (player.moveInput.x != 0 && Mathf.Sign(player.moveInput.x) != player.FacingDirection)
        {
            player.Flip();
        }

        if (player.inputActions.MoveSystem.Jump.WasPressedThisFrame() && player.jumpTime < 2)
        {
            stateMachine.ChangeState(player.jumpState);
        }

    }

    public override void Exit()
    {
        base.Exit();
    }
}
