using UnityEngine;
using Cinemachine;

public class CamreaControl : MonoBehaviour
{
    CinemachineConfiner2D confiner2D;
    // 获取脉冲源
    public CinemachineImpulseSource impulseSource;
    public VoidSo CameraShakeEvent;// 获取刚刚写好的事件脚本
    public VoidSo SceneAfterEvent; //这是场景加载之后的事件脚本
    private void Awake() 
    {
        confiner2D = GetComponent<CinemachineConfiner2D>();
        // impulseSource = GetComponent<CinemachineImpulseSource>();
    }
    private void OnEnable() 
    {
        CameraShakeEvent.OnEventRaised += Onshake;   
        SceneAfterEvent.OnEventRaised += OnFindBound;
    }
    private void OnDisable() 
    {
        CameraShakeEvent.OnEventRaised -= Onshake;    
        SceneAfterEvent.OnEventRaised -= OnFindBound;
    }
    private void OnFindBound()
    {
        findCameraBounds();
    }

    /// <summary>
    /// 实现震动效果（官方函数）
    /// </summary>
    private void Onshake()
    {
        impulseSource.GenerateImpulse();
    }

      /// <summary>
    /// 找到这个场景中的边界碰撞体
    /// </summary>
    public void findCameraBounds()
    {
        var obj = GameObject.FindGameObjectWithTag("Bounds");
        if(obj == null)
            return;
        
        confiner2D.m_BoundingShape2D = obj.GetComponent<Collider2D>();

        // 在新加载出一个场景的边界的时候，要先清除上一个边界的形状缓存，才能应用下一个边界
        confiner2D.InvalidateCache();
    }

}
