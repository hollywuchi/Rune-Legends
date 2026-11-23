using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 随机选择节点（加权随机 / 洗牌随机）：
/// 每次重新进入时随机生成子节点的尝试顺序，直到有子节点返回 Running 或 Success。
/// </summary>
public class RandomSelectorNode : CompositeNode
{
    [Header("权重配置（可选，长度不足或未配置则默认等概率随机）")]
    public List<float> weights = new();

    private readonly List<int> executionOrder = new();
    private int currentOrderIndex = 0;

    protected override void OnStart()
    {
        currentOrderIndex = 0;
        BuildRandomOrder();
    }

    protected override BtLifeTimeState OnUpdate()
    {
        if (children == null || children.Count == 0) return BtLifeTimeState.Failure;

        while (currentOrderIndex < executionOrder.Count)
        {
            int childIndex = executionOrder[currentOrderIndex];
            var state = children[childIndex].Evaluate();

            switch (state)
            {
                case BtLifeTimeState.Running:
                    return BtLifeTimeState.Running;
                case BtLifeTimeState.Success:
                    return BtLifeTimeState.Success;
                case BtLifeTimeState.Failure:
                    currentOrderIndex++;
                    break;
            }
        }

        return BtLifeTimeState.Failure;
    }

    protected override void OnStop()
    {
        currentOrderIndex = 0;
    }

    private void BuildRandomOrder()
    {
        executionOrder.Clear();
        if (children == null || children.Count == 0) return;

        List<int> pool = new List<int>(children.Count);
        for (int i = 0; i < children.Count; i++)
        {
            pool.Add(i);
        }

        bool hasValidWeights = weights != null && weights.Count == children.Count;

        while (pool.Count > 0)
        {
            int selectedPoolIndex;
            if (hasValidWeights)
            {
                float totalWeight = 0f;
                for (int i = 0; i < pool.Count; i++)
                {
                    totalWeight += Mathf.Max(0.001f, weights[pool[i]]);
                }

                float randomRoll = Random.Range(0f, totalWeight);
                float currentSum = 0f;
                selectedPoolIndex = pool.Count - 1;

                for (int i = 0; i < pool.Count; i++)
                {
                    currentSum += Mathf.Max(0.001f, weights[pool[i]]);
                    if (randomRoll <= currentSum)
                    {
                        selectedPoolIndex = i;
                        break;
                    }
                }
            }
            else
            {
                selectedPoolIndex = Random.Range(0, pool.Count);
            }

            executionOrder.Add(pool[selectedPoolIndex]);
            pool.RemoveAt(selectedPoolIndex);
        }
    }
}
