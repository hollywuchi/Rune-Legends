using UnityEngine;

namespace BehaviorTree.Leaves.Condition
{
    /// <summary>
    /// 检查 Boss 与目标玩家的水平距离是否在指定区间内。
    /// </summary>
    public class CheckDistanceBandNode : ActionNode
    {
        public enum DistanceBand
        {
            Custom = 0,
            Close = 1,
            Mid = 2,
            Far = 3
        }

        [Tooltip("优先使用 EnemyConfig 中的统一距离分段；Custom 则使用下方的 minDistance/maxDistance")]
        public DistanceBand band = DistanceBand.Custom;

        [Tooltip("最小水平距离（包含，仅 Custom 模式使用）")]
        public float minDistance = 0f;

        [Tooltip("最大水平距离（包含，仅 Custom 模式使用）")]
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

            bool inBand;
            if (band != DistanceBand.Custom && config != null)
            {
                inBand = band switch
                {
                    DistanceBand.Close => distanceX <= config.closeRange,
                    DistanceBand.Mid => distanceX > config.closeRange && distanceX <= config.midRange,
                    DistanceBand.Far => distanceX > config.midRange,
                    _ => false
                };
            }
            else
            {
                inBand = distanceX >= minDistance && distanceX <= maxDistance;
            }

            return inBand ? BtLifeTimeState.Success : BtLifeTimeState.Failure;
        }

        protected override void OnStop()
        {
        }
    }
}
