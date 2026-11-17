using UnityEngine;

namespace BehaviorTree.Leaves.Condition
{
    public class CheckAttackRangeNode : ActionNode
    {
        [Header("攻击检测偏移与范围")]
        public Vector2 offset;
        public Vector2 checkSize;

        protected override void OnStart()
        {
        }

        protected override BtLifeTimeState OnUpdate()
        {
            if (blackBoard == null) return BtLifeTimeState.Failure;

            // 还没有锁定玩家，不满足攻击前提
            if (blackBoard.TargetPlayer == null)
                return BtLifeTimeState.Failure;

            // 检测目标玩家是否进入面前的攻击框
            bool inRange = context.Motor.CheckAttackRange(offset, checkSize);

            // 在攻击范围内返回Success触发后续攻击，不在则返回Failure
            // 注意：绝不修改黑板上的 TargetPlayer，保持追击目标不丢失！
            return inRange ? BtLifeTimeState.Success : BtLifeTimeState.Failure;
        }

        protected override void OnStop()
        {
        }
    }
}
