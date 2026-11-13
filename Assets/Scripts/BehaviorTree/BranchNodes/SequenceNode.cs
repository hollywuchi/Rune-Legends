using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 顺序节点：先序遍历，所有节点顺序执行
/// 遍历节点，直到所有节点返回Success
/// </summary>
public class SequenceNode : CompositeNode
{
    private int _currentChildIndex;

    protected override void OnStart()
    {
        _currentChildIndex = 0;
    }

    protected override BtLifeTimeState OnUpdate()
    {
        if (children.Count == 0) return BtLifeTimeState.Failure;
        for (int i = _currentChildIndex; i < children.Count; i++)
        {
            var state = children[i].Evaluate();
            if (state == BtLifeTimeState.Running) _currentChildIndex = i;
            if(state != BtLifeTimeState.Success) return state;
        }

        return BtLifeTimeState.Success;
    }

    protected override void OnStop()
    {
        
    }
}