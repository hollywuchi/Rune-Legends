using UnityEngine;
using UnityEngine.Pool;

public class Effect_Elements : MonoBehaviour
{
    private ObjectPool<GameObject> pool;
    private ParticleSystem ps;
    private EventBinding<ReliseFxEvent> reliseFxEvent;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void OnEnable()
    {
        ConfigureParticleSystem();
        reliseFxEvent = new EventBinding<ReliseFxEvent>(ReleasePool);

        EventBus<ReliseFxEvent>.Register(reliseFxEvent);
    }

    void OnDisable()
    {
        EventBus<ReliseFxEvent>.Deregister(reliseFxEvent);
    }

    void OnParticleSystemStopped()
    {
        ReleasePool();
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
        if(!gameObject.activeSelf) return;
        
        if (pool != null)
            pool.Release(gameObject);
        else
            Destroy(gameObject);
    }
}
