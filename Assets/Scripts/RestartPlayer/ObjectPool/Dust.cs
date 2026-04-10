using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Dust : MonoBehaviour
{
    private ObjectPool<GameObject> pool;

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
}
