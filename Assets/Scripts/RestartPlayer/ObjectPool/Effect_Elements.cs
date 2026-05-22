using UnityEngine;
using UnityEngine.Pool;

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
