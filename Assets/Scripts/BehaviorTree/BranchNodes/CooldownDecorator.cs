using UnityEngine;
using UnityEngine.UIElements;

public class CooldownDecorator : DecoratorNode
{
    public float cooldownTime;
    private float nextAvailableTime;
    
    protected override void OnStart()
    {
    }

    protected override BtLifeTimeState OnUpdate()
    {
        if (Time.time < nextAvailableTime) return BtLifeTimeState.Failure;

        var state = child.Evaluate();
        if (state == BtLifeTimeState.Success)
        {
            nextAvailableTime = Time.time + cooldownTime;
        }

        return BtLifeTimeState.Success;
    }

    protected override void OnStop()
    {
    }

    public override BtBaseNode Clone()
    {
        var node = Instantiate(this);
        if (child != null) node.child = child.Clone();
        node.nextAvailableTime = 0;
        return node;
    }
}