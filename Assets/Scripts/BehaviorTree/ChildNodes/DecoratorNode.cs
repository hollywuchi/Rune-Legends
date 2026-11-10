using UnityEngine;

public abstract class DecoratorNode : BtBaseNode
{
    [HideInInspector] public BtBaseNode child;
    
    protected override void OnStart()
    {
        throw new System.NotImplementedException();
    }

    protected override BtLifeTimeState OnUpdate()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnStop()
    {
        throw new System.NotImplementedException();
    }

    public override BtBaseNode Clone()
    {
        DecoratorNode node = Instantiate(this);
        if (child != null) node.child = child.Clone();
        return node;
    }
}