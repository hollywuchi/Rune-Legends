using UnityEngine;

namespace BehaviorTree.Leaves.Condition
{
    /// <summary>
    /// 检查 Boss 与目标玩家的水平距离是否在指定区间内。
    /// </summary>
    public class CheckDistanceBandNode : ActionNode
    {
        [Tooltip("最小水平距离（包含）")]
        public float minDistance = 0f;

        [Tooltip("最大水平距离（包含）")]
        public float maxDistance = 3f;

        protected override void OnStart()
        {
        }

        protected override BtLifeTimeState OnUpdate()
        {
            if (blackBoard == null || blackBoard.TargetPlayer == null || context == null || context.Transform == null)
            {
                return BtLifeTimeState.Failure;
            }

            float distanceX = Mathf.Abs(context.Transform.position.x - blackBoard.TargetPlayer.position.x);
            bool inBand = distanceX >= minDistance && distanceX <= maxDistance;
            return inBand ? BtLifeTimeState.Success : BtLifeTimeState.Failure;
        }

        protected override void OnStop()
        {
        }
    }
}
