using System.Collections.Generic;
using UnityEngine;

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