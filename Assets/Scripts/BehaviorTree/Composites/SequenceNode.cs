using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            var state = children[i].Evalucate();
            if (state == BtLifeTimeState.Running) _currentChildIndex = i;
            if(state != BtLifeTimeState.Success) return state;
        }

        return BtLifeTimeState.Success;
    }

    protected override void OnStop()
    {
        
    }
}