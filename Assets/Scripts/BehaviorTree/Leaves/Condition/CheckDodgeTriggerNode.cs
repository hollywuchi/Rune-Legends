using UnityEngine;

public class CheckDodgeTriggerNode : ActionNode
{
    private float maxTriggerDistance;
    private PlayerContext playerContext;

    protected override void OnStart()
    {
        playerContext = blackBoard.TargetPlayer.GetComponent<Player>()?.context;
    }

    protected override BtLifeTimeState OnUpdate()
    {
        if (context.PostureSystem.isBroken) return BtLifeTimeState.Failure;

        var distance = Mathf.Abs(blackBoard.TargetPosition.x - context.Transform.position.x);
        var canDodge = playerContext is { IsLightCutting: true } && distance < maxTriggerDistance;

        return canDodge ? BtLifeTimeState.Success : BtLifeTimeState.Failure;

        // TODO：继续尝试实现BOSS的行为树
    }

    protected override void OnStop()
    {
    }
}