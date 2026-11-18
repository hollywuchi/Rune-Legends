using UnityEngine;

public class CheckDistanceBandNode : ActionNode
{
    private float minDistance;
    private float maxDistance;

    protected override void OnStart()
    {
    }

    protected override BtLifeTimeState OnUpdate()
    {
        var distance = Mathf.Abs(context.Transform.position.x - blackBoard.TargetPlayer.position.x);

        if (distance < minDistance)
        {
            // 这里放近距离会干的事
        }
        else if (distance > minDistance && distance < maxDistance)
        {
            // 这里放中距离会干的事
        }
        else
        {
            // 这里放远距离干的事   
        }

        return BtLifeTimeState.Success;
    }

    protected override void OnStop()
    {
    }
}