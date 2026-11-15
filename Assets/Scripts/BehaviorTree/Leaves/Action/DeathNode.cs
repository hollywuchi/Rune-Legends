public class DeathNode : ActionNode
{
    protected override void OnStart()
    {
        if (context.Character == null || !(context.Character.CurrentHealth <= 0)) return;
        context.Motor.Stop();
        context.Anim.SetBool(EnemyAnimHush.Death, true);
    }

    protected override BtLifeTimeState OnUpdate()
    {
        return context.Character.CurrentHealth <= 0 ? BtLifeTimeState.Running : BtLifeTimeState.Failure;
    }

    protected override void OnStop()
    {
    }
}