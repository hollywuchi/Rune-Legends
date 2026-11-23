namespace BehaviorTree.Leaves.Condition
{
    /// <summary>
    /// 检查 Boss 是否尚未播放开场动画。
    /// 若尚未播放则返回 Success 进入出场分支；已播放则返回 Failure。
    /// </summary>
    public class CheckBossIntroNode : ActionNode
    {
        protected override void OnStart()
        {
        }

        protected override BtLifeTimeState OnUpdate()
        {
            if (blackBoard == null) return BtLifeTimeState.Failure;
            return !blackBoard.HasPlayedIntro ? BtLifeTimeState.Success : BtLifeTimeState.Failure;
        }

        protected override void OnStop()
        {
        }
    }
}
