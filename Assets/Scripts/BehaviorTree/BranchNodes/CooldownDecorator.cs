using UnityEngine;

/// <summary>
/// 冷却装饰器节点：
/// 包装在技能/动作节点外部。当处于冷却中时返回 Failure，让父级 Selector 顺延执行其他招式；
/// 当子节点执行成功（或配置失败也冷却）后刷新下一次可用时间。
/// </summary>
public class CooldownDecorator : DecoratorNode
{
    [Header("冷却时间（秒）")]
    public float cooldownTime = 5f;

    [Tooltip("子节点执行失败时是否也计入冷却，默认 false（只有成功出招才进冷却）")]
    public bool triggerCooldownOnFailure = false;

    private float nextAvailableTime = 0f;

    protected override void OnStart()
    {
    }

    protected override BtLifeTimeState OnUpdate()
    {
        if (child == null) return BtLifeTimeState.Failure;

        if (Time.time < nextAvailableTime)
        {
            return BtLifeTimeState.Failure;
        }

        var state = child.Evaluate();

        if (state == BtLifeTimeState.Success || (state == BtLifeTimeState.Failure && triggerCooldownOnFailure))
        {
            nextAvailableTime = Time.time + cooldownTime;
        }

        return state;
    }

    protected override void OnStop()
    {
    }

    public override BtBaseNode Clone()
    {
        CooldownDecorator clone = (CooldownDecorator)base.Clone();
        clone.nextAvailableTime = 0f;
        return clone;
    }
}
