using UnityEngine;

namespace BehaviorTree.Leaves.Condition
{
    /// <summary>
    /// 闪避反击直觉检测节点：
    /// 检测玩家是否正在施展【流光闪击】或贴身普通攻击且在危险距离内；
    /// 当 Boss 处于破防或弹反硬直中时绝不触发闪避。
    /// </summary>
    public class CheckDodgeTriggerNode : ActionNode
    {
        [Header("触发距离判定")]
        [Tooltip("触发闪避的最大水平距离（超出此距离不会盲目闪避）")]
        public float maxTriggerDistance = 5.5f;

        [Header("行为捕捉开关")]
        [Tooltip("检测玩家流光闪击（霹雳一闪）")]
        public bool detectLightCut = true;

        [Tooltip("是否在玩家蓄力阶段就预警（false 则在玩家松开按键冲出瞬间闪避）")]
        public bool triggerOnCharging = false;

        [Tooltip("检测玩家普通贴身攻击")]
        public bool detectNormalAttack = true;

        [Tooltip("普通攻击触发闪避的最大距离")]
        public float meleeAttackTriggerDistance = 2.5f;

        [Header("触发概率")]
        [Range(0f, 1f)] public float triggerChance = 1.0f;

        protected override void OnStart()
        {
        }

        protected override BtLifeTimeState OnUpdate()
        {
            if (blackBoard == null || blackBoard.TargetPlayer == null || context == null || context.Transform == null)
            {
                return BtLifeTimeState.Failure;
            }

            // 破防与弹反硬直保护
            if (context.PostureSystem != null && context.PostureSystem.isBroken)
                return BtLifeTimeState.Failure;

            if (context.Motor != null && context.Motor.IsBlocked)
                return BtLifeTimeState.Failure;

            float distanceX = Mathf.Abs(context.Transform.position.x - blackBoard.TargetPlayer.position.x);
            if (distanceX > maxTriggerDistance)
                return BtLifeTimeState.Failure;

            PlayerContext pContext = blackBoard.TargetPlayer.GetComponent<Player>()?.context;
            if (pContext == null)
                return BtLifeTimeState.Failure;

            bool shouldTrigger = false;

            if (detectLightCut)
            {
                if (pContext.IsLightCutting)
                {
                    shouldTrigger = true;
                }
                else if (triggerOnCharging && pContext.IsLightCutCharging && distanceX <= maxTriggerDistance)
                {
                    shouldTrigger = true;
                }
            }

            if (!shouldTrigger && detectNormalAttack)
            {
                if (pContext.IsAttacking && distanceX <= meleeAttackTriggerDistance)
                {
                    shouldTrigger = true;
                }
            }

            if (!shouldTrigger)
                return BtLifeTimeState.Failure;

            if (triggerChance < 1f && Random.value > triggerChance)
                return BtLifeTimeState.Failure;

            return BtLifeTimeState.Success;
        }

        protected override void OnStop()
        {
        }
    }
}
