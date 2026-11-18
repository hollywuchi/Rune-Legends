using System.Collections.Generic;
using System.Diagnostics;

public class RandomSelectorNode : CompositeNode
{
    private List<int> executionOrder;
    private int currentOrderIndex;
    
    protected override void OnStart()
    {
        // 根据洗牌算法进行随机
    }

    protected override BtLifeTimeState OnUpdate()
    {
        if (children.Count == 0) return BtLifeTimeState.Failure;

        for (var i = currentOrderIndex; i < children.Count; i++)
        {
            var state = children[executionOrder[currentOrderIndex]].Evaluate();

            switch (state)
            {
                case BtLifeTimeState.Running:
                    return BtLifeTimeState.Running;
                case BtLifeTimeState.Success:
                    return BtLifeTimeState.Success;
                case BtLifeTimeState.Failure:
                    currentOrderIndex++;
                    break;
            }
        }

        return BtLifeTimeState.Failure;
    }

    protected override void OnStop()
    {
    }
}