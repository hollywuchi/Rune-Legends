public class ReactiveSequenceNode : CompositeNode
{
    protected override void OnStart()
    {
    }

    protected override BtLifeTimeState OnUpdate()
    {
        if (children.Count == 0) return BtLifeTimeState.Failure;
        for (var i = 0; i < children.Count; i++)
        {
            var state = children[i].Evaluate();
            if (state == BtLifeTimeState.Success) continue;
            for (var j = i + 1; j < children.Count; j++)
            {
                if(children[j].Started)
                    children[j].Abort();
            }

            return state;
        }

        return BtLifeTimeState.Success;
    }

    protected override void OnStop()
    {
    }
}