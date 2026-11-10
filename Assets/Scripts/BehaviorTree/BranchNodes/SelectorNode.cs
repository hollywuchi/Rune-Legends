using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 选择节点：先序遍历，最重要的先执行
/// 遍历节点，直到有节点返回Success
/// </summary>
public class SelectorNode : CompositeNode
{
    private int _currentChildren;
    
    protected override void OnStart()
    {
        _currentChildren = 0;
    }

    protected override BtLifeTimeState OnUpdate()
    {
        if (children.Count == 0) return BtLifeTimeState.Failure;
        for (int i = _currentChildren; i < children.Count; i++)
        {
            var _State = children[i].Evalucate();
            if (_State == BtLifeTimeState.Running) _currentChildren = i;
            if (_State != BtLifeTimeState.Failure) return _State;
        }

        return BtLifeTimeState.Failure;
    }

    protected override void OnStop()
    {
        
    }
}