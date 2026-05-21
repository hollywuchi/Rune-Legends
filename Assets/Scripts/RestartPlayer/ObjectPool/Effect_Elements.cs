using UnityEngine;
using UnityEngine.Pool;

// TODO-- 特效现在只会朝着一个方向发射
// TODO-- 特效现在生成的频率过高，与玩家的攻击频率不符
// TODO-- 玩家现在的攻击不会产生击退效果
public class Effect_Elements : MonoBehaviour
{
    private ObjectPool<GameObject> pool;
    private ParticleSystem ps;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        ConfigureParticleSystem();
    }

    void OnEnable()
    {
        ConfigureParticleSystem();
    }

    private void ConfigureParticleSystem()
    {
        if (ps != null)
        {
            var main = ps.main;
            main.stopAction = ParticleSystemStopAction.Callback;
        }
    }

    public void SetPool(ObjectPool<GameObject> pool)
    {
        this.pool = pool;
    }

    public void ReleasePool()
    {
        if (pool != null)
            pool.Release(gameObject);
        else 
            Destroy(gameObject);
    }

    void OnParticleSystemStopped()
    {
        ReleasePool();
    }
}
