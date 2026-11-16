using UnityEngine;

public class WaitNode : ActionNode
{
    private float waitingTimer;
    public float waitTime;

    protected override void OnStart()
    {
        waitingTimer = waitTime;
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