public class CheckHurtNode : ActionNode
{
    protected override void OnStart()
    {
    }

    protected override BtLifeTimeState OnUpdate()
    {
        return context.Character.isHurt ? BtLifeTimeState.Success : BtLifeTimeState.Failure;
    }

    protected override void OnStop()
    {
    }
}