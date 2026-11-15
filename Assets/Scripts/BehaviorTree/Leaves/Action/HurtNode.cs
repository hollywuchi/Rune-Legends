using UnityEngine;

public class HurtNode : ActionNode
{
    protected override void OnStart()
    {
        context.Anim.SetTrigger(EnemyAnimHush.Hurt);
    }

    protected override BtLifeTimeState OnUpdate()
    {
        return !context.Character.isHurt ? BtLifeTimeState.Success : BtLifeTimeState.Running;
    }

    protected override void OnStop()
    {
        context.Motor.FaceTarget(context.Motor.attacker);
    }
}