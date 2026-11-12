using UnityEngine;

/// <summary>
/// 测试节点，并无实际作用
/// </summary>
public class ChaseNode : ActionNode
{
    private bool isFindPlayer;

    protected override void OnStart()
    {
        isFindPlayer = context.enemy.FindPlayer();
    }

    protected override BtLifeTimeState OnUpdate()
    {
        if (isFindPlayer)
        {
            Chase();
            return BtLifeTimeState.Running;
        }

        return BtLifeTimeState.Success;

    }

    protected override void OnStop()
    {
    }

    private void Chase()
    {
        context.rb.velocity = new Vector3(context.enemy.chaseSpeed * Time.deltaTime, context.rb.velocity.y, 0);
    }
}