using System;
using BehaviorTree.Core;
using UnityEngine;

public class BehaviorTreeRunner : MonoBehaviour
{
    public bool isInto = true;
    public BehaviorTreeSO treeAssets;
    [HideInInspector] public BehaviorTreeSO _runtimeTree;
    
    private void Start()
    {
        var context = EnemyContext.CreateFromObject(gameObject);
        _runtimeTree = treeAssets.Clone();
        _runtimeTree.Bind(context);
    }

    private void Update()
    {
        if (isInto)
            _runtimeTree.UpdateTree();
    }

    public void DestroySelf() => Destroy(gameObject);
}