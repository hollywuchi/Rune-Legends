public class BlockNode : ActionNode
{
    private bool playedBlockAnim;
    private bool playedBrokenAnim;

    protected override void OnStart()
    {
        playedBlockAnim = false;
        playedBrokenAnim = false;
    }

    protected override BtLifeTimeState OnUpdate()
    {
        var isBroken = context.Motor != null && context.PostureSystem.isBroken;
        var isBlock = context.Motor != null && context.Motor.IsBlocked;

        if (!isBlock && !isBroken) return BtLifeTimeState.Failure;

        if (isBroken)
        {
            if (!playedBrokenAnim)
            {
                playedBrokenAnim = true;
                context.Anim.SetTrigger(EnemyAnimHush.Hurt);
                context.Anim.SetBool(EnemyAnimHush.IsBroken, true);
            }

            return BtLifeTimeState.Running;
        }

        if (!playedBlockAnim)
        {
            playedBlockAnim = true;
            context.Anim.SetTrigger(EnemyAnimHush.Hurt);
        }

        return BtLifeTimeState.Running;
    }

    protected override void OnStop()
    {
        if (context.Anim != null)
            context.Anim.SetBool(EnemyAnimHush.IsBroken, false);
    }
}