public abstract class ReactiveSequenceNode : CompositeNode
{
    protected override BtLifeTimeState OnUpdate()
    {
        if (children.Count == 0) return BtLifeTimeState.Failure;
        foreach (var t in children)
        {
            var state = t.Evaluate();
            if(state != BtLifeTimeState.Success) return state;
        }

        return BtLifeTimeState.Success;
    }
}