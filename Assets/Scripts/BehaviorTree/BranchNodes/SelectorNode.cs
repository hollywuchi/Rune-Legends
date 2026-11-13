using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 选择节点：先序遍历，最重要的先执行
/// 遍历节点，直到有节点返回Success
/// </summary>
public class SelectorNode : CompositeNode
{
    private int _currentChildren;
    private int _lastRunningIndex;

    protected override void OnStart()
    {
        _currentChildren = 0;
        _lastRunningIndex = -1;
    }

    protected override BtLifeTimeState OnUpdate()
    {
        if (children.Count == 0) return BtLifeTimeState.Failure;
        for (int i = 0; i < children.Count; i++)
        {
            var _State = children[i].Evaluate();
            if (_State != BtLifeTimeState.Failure)
            {
                // 如果上一帧还在跑比当前优先级更低的旧分支，那么就先打断他
                if (_lastRunningIndex != -1 && _lastRunningIndex > i)
                {
                    children[_lastRunningIndex].Abort();
                }

                _lastRunningIndex = _State == BtLifeTimeState.Running ? i : -1;
                return _State;
            }
        }

        _lastRunningIndex = -1;
        return BtLifeTimeState.Failure;
    }

    protected override void OnStop()
    {
    }
}