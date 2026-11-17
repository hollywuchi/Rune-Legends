using UnityEngine;

namespace BehaviorTree.Leaves.Condition
{
    public class CheckTargetInRangeNode : ActionNode
    {
        [Header("未发现目标时的前方视野偏移与大小")]
        public Vector2 offset;
        public Vector2 checkSize;

        private float loseTargetTimer;

        protected override void OnStart()
        {
        }

        protected override BtLifeTimeState OnUpdate()
        {
            if (blackBoard == null) return BtLifeTimeState.Failure;

            context.Motor.offset = offset;
            context.Motor.checkSizeFinding = checkSize;

            // 1. 未发现目标时：用前方矩形视野（FindPlayer）
            if (blackBoard.TargetPlayer == null)
            {
                var detectedPlayer = context.Motor.FindPlayer();
                if (detectedPlayer != null)
                {
                    blackBoard.TargetPlayer = detectedPlayer;
                    blackBoard.TargetPosition = detectedPlayer.position;
                    loseTargetTimer = config != null ? config.loseTargetTime : 3f;
                    blackBoard.LoseTargetTimer = loseTargetTimer;
                    blackBoard.IsVision = true;
                    return BtLifeTimeState.Success;
                }

                blackBoard.IsVision = false;
                return BtLifeTimeState.Failure;
            }

            // 2. 已发现目标（警戒中）：改用全向圆形警戒圈（FoundPlayer）跟踪
            var alertPlayer = context.Motor.FoundPlayer();
            if (alertPlayer != null)
            {
                // 玩家还在警戒圈内（包括跳到身后的情况）
                blackBoard.TargetPlayer = alertPlayer;
                blackBoard.TargetPosition = alertPlayer.position;
                loseTargetTimer = config != null ? config.loseTargetTime : 3f;
                blackBoard.LoseTargetTimer = loseTargetTimer;
                blackBoard.IsVision = true;
                return BtLifeTimeState.Success;
            }

            // 3. 玩家离开了警戒圈：开启脱战倒计时
            blackBoard.IsVision = false;
            loseTargetTimer -= Time.deltaTime;
            blackBoard.LoseTargetTimer = loseTargetTimer;

            if (loseTargetTimer > 0f)
            {
                // 未超时：保持追击状态（继续追向最后位置）
                return BtLifeTimeState.Success;
            }

            // 4. 超时：彻底脱战，清空目标，返回Failure退出追击分支
            blackBoard.TargetPlayer = null;
            return BtLifeTimeState.Failure;
        }

        protected override void OnStop()
        {
        }
    }
}
