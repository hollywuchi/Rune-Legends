using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeNode : BtBaseNode
{

    public List<BtBaseNode> children = new();
    
    public override BtBaseNode Clone()
    {
        CompositeNode node = Instantiate(this);
        
        node.children.Clear();
        foreach (var item in children)
            if (item != null)
                node.children.Add(item.Clone());
        return node;
    }
}