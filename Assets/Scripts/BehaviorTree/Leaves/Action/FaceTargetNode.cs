/// <summary>
/// 面向目标玩家节点：
/// 配合 WaitNode 使用，在停顿或喘息后转向面对玩家。
/// </summary>
public class FaceTargetNode : ActionNode
{
    protected override void OnStart()
    {
        if (context != null && context.Motor != null && blackBoard != null && blackBoard.TargetPlayer != null)
        {
            context.Motor.FaceTarget(blackBoard.TargetPlayer);
        }
    }

    protected override BtLifeTimeState OnUpdate()
    {
        return BtLifeTimeState.Success;
    }

    protected override void OnStop()
    {
    }
}
