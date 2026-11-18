public class CheckHealthRatioNode : ActionNode
{
    public float maxRatio;
    
    protected override void OnStart()
    {
    }

    protected override BtLifeTimeState OnUpdate()
    {
        return context.Character.currentHealth / context.Character.maxHealth > maxRatio
            ? BtLifeTimeState.Success
            : BtLifeTimeState.Failure;
    }

    protected override void OnStop()
    {
    }
}