namespace BehaviorTree.Leaves.Action
{
    public class AttackNode : ActionNode
    {
        protected override void OnStart()
        {
            context.Motor.attackFinished = false;
            context.Motor.Stop();
            context.Anim.SetTrigger(EnemyAnimHush.Attack);
            context.Motor.FaceTarget(blackBoard.targetPlayer);
        }

        protected override BtLifeTimeState OnUpdate()
        {
            return context.Motor.attackFinished ? BtLifeTimeState.Success : BtLifeTimeState.Running;
        }

        protected override void OnStop()
        {
            context.Motor.attackFinished = false;
        }
    }
}