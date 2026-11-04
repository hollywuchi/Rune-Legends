using System;
using UnityEngine;
using Cinemachine;
using UnityEngine.Serialization;

public class CameraControl : MonoBehaviour
{
    private CinemachineConfiner2D confiner2D;

    // 获取脉冲源
    public CinemachineImpulseSource blockImpulseSource;
    public CinemachineImpulseSource heavyAtkImpulseSource;

    private EventBinding<BlockCameraShakeEvent> blockCameraEventBinding;
    private EventBinding<HeavyAtkCameraShakeEvent> heavyAtkCameraShakeEventBinding;
    private EventBinding<AfterSceneLoadEvent> afterSceneLoadEventBinding;

    private void Awake()
    {
        confiner2D = GetComponent<CinemachineConfiner2D>();
        if (confiner2D != null) OnFindBound();
    }

    private void OnEnable()
    {
        blockCameraEventBinding = new EventBinding<BlockCameraShakeEvent>(OnBlockShake);
        afterSceneLoadEventBinding = new EventBinding<AfterSceneLoadEvent>(OnFindBound);
        heavyAtkCameraShakeEventBinding = new EventBinding<HeavyAtkCameraShakeEvent>(OnHeavyAtkShake);

        EventBus<BlockCameraShakeEvent>.Register(blockCameraEventBinding);
        EventBus<AfterSceneLoadEvent>.Register(afterSceneLoadEventBinding);
        EventBus<HeavyAtkCameraShakeEvent>.Register(heavyAtkCameraShakeEventBinding);
    }

    private void OnDisable()
    {
        EventBus<BlockCameraShakeEvent>.Deregister(blockCameraEventBinding);
        EventBus<AfterSceneLoadEvent>.Deregister(afterSceneLoadEventBinding);
        EventBus<HeavyAtkCameraShakeEvent>.Deregister(heavyAtkCameraShakeEventBinding);
    }

    private void OnFindBound()
    {
        FindCameraBounds();
    }

    /// <summary>
    /// 实现震动效果（官方函数）
    /// </summary>
    private void OnBlockShake() => blockImpulseSource.GenerateImpulse();
    private void OnHeavyAtkShake() => heavyAtkImpulseSource.GenerateImpulse();

    /// <summary>
    /// 找到这个场景中的边界碰撞体
    /// </summary>
    private void FindCameraBounds()
    {
        var obj = GameObject.FindGameObjectWithTag("Bounds");
        if (obj == null) return;

        confiner2D.m_BoundingShape2D = obj.GetComponent<Collider2D>();

        // 在新加载出一个场景的边界的时候，要先清除上一个边界的形状缓存，才能应用下一个边界
        confiner2D.InvalidateCache();
    }
}