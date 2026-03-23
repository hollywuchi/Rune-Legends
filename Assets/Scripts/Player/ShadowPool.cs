using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class ShadowPool : MonoBehaviour
{
    public static ShadowPool instance;
    public GameObject ShadowPrefabs;
    public int ShadowCount;
    Queue<GameObject> availableObjects = new Queue<GameObject>();

    private void Awake() 
    {
        instance = this;

        // 初始化对象池 
        FillPool();
    }

    void FillPool()
    {
        for(int i = 0;i <= ShadowCount; i++)
        {
            var Shadow = Instantiate(ShadowPrefabs,transform);
            ReturenPool(Shadow);
        }
    }
    public void ReturenPool(GameObject gameObject)
    {
        // 返回对象池
        gameObject.SetActive(false);
        availableObjects.Enqueue(gameObject);

    }
    public GameObject GetFromPool()
    {
        if(availableObjects.Count == 0)
        {
            FillPool();
        }
        var outshow = availableObjects.Dequeue();
        outshow.SetActive(true);
        return outshow;
    }
}
