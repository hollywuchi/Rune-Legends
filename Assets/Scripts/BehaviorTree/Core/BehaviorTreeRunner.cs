using System;
using BehaviorTree.Core;
using UnityEngine;

public class BehaviorTreeRunner : MonoBehaviour
{
    public BehaviorTreeSO treeAssets;
    private BehaviorTreeSO _runtimeTree;

    private void Start()
    {
        var context = EnemyContext.CreateFromObject(gameObject);
        _runtimeTree = treeAssets.Clone();
        _runtimeTree.Bind(context);
    }

    private void Update()
    {
        _runtimeTree.UpdateTree();
    }
}