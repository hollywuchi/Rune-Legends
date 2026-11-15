using UnityEngine;

public abstract class DecoratorNode : BtBaseNode
{
    [HideInInspector] public BtBaseNode child;
    
    public override BtBaseNode Clone()
    {
        DecoratorNode node = Instantiate(this);
        if (child != null) node.child = child.Clone();
        return node;
    }
}