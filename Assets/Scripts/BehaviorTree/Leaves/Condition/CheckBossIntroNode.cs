namespace BehaviorTree.Leaves.Action
{
    public class CheckBossIntroNode  :ActionNode
    {
        protected override void OnStart()
        {
        }

        protected override BtLifeTimeState OnUpdate()
        {
            return !blackBoard.HasPlayedIntro ? BtLifeTimeState.Success : BtLifeTimeState.Failure;
        }

        protected override void OnStop()
        {
        }
    }
}