using UnityEngine;

public class WaitNode : ActionNode
{
    private float waitingTimer;

    protected override void OnStart()
    {
        waitingTimer = config != null ? config.waitingTime : 1f;
    }

    protected override BtLifeTimeState OnUpdate()
    {
        if (waitingTimer > 0)
        {
            waitingTimer -= Time.deltaTime;
            return BtLifeTimeState.Running;
        }

        return BtLifeTimeState.Success;
    }

    protected override void OnStop()
    {
    }
}