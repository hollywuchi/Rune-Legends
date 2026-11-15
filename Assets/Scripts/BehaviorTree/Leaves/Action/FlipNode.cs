using UnityEngine;

public class FlipNode : ActionNode
{
    protected override void OnStart()
    {
        context.Motor.Flip();
    }

    protected override BtLifeTimeState OnUpdate()
    {
        return BtLifeTimeState.Success;
    }

    protected override void OnStop()
    {
    }
}