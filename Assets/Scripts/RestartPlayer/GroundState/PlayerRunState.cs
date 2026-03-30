using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerGroundState
{
    public PlayerRunState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        player.animator.Play("ToRun");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Run();

        if (Mathf.Abs(player.moveInput.x) < 0.1f)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.animator.Play("BreakRun");
    }

    private void Run()
    {
        // player.inputActions.MoveSystem.WalkOrRun
        Vector2 dir = player.inputActions.MoveSystem.WalkOrRun.ReadValue<Vector2>();

        player.rb.velocity = new Vector2(player.moveInput.x + dir.x * player.Speed * Time.deltaTime, player.rb.velocity.y);
    }

    
}