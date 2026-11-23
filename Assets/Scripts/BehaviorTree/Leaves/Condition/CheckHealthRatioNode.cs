using UnityEngine;

namespace BehaviorTree.Leaves.Condition
{
    /// <summary>
    /// 检查 Boss 当前生命值比例是否在 [minRatio, maxRatio] 区间内。
    /// 例如：二阶段配置 [0, 0.5]，一阶段配置 [0.5, 1]。
    /// </summary>
    public class CheckHealthRatioNode : ActionNode
    {
        [Range(0f, 1f)] public float minRatio = 0f;
        [Range(0f, 1f)] public float maxRatio = 0.5f;

        protected override void OnStart()
        {
        }

        protected override BtLifeTimeState OnUpdate()
        {
            if (context == null || context.Character == null) return BtLifeTimeState.Failure;

            float maxHealth = context.Character.maxHealth;
            if (maxHealth <= 0f) return BtLifeTimeState.Failure;

            float currentRatio = context.Character.currentHealth / maxHealth;
            bool inRange = currentRatio >= minRatio && currentRatio <= maxRatio;
            return inRange ? BtLifeTimeState.Success : BtLifeTimeState.Failure;
        }

        protected override void OnStop()
        {
        }
    }
}
