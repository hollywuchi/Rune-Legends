using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    private ObjectPool<GameObject> pool;        // 为了初始化对象池，先创建一个对象池的变量
    public List<GameObject> poolPrefabs;
    // 对象池的列表
    private List<ObjectPool<GameObject>> poolEffectList = new List<ObjectPool<GameObject>>();
    // private Queue<GameObject> soundQueue = new Queue<GameObject>();     // 队列，当做音效的对象池

    void Awake()
    {
        CreatePool();
    }

    private void CreatePool()
    {
        foreach (GameObject item in poolPrefabs)
        {
            Transform parent = new GameObject(item.name).transform;
            parent.SetParent(transform);

            pool = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    GameObject obj = Instantiate(item, parent);
                    obj.GetComponent<Dust>().SetPool(pool);
                    return obj;
                },
                actionOnGet: obj => obj.SetActive(true),
                actionOnRelease: obj => obj.SetActive(false),
                actionOnDestroy: obj => Destroy(obj),
                defaultCapacity: 5,
                maxSize: 30
            );

            poolEffectList.Add(pool);
        }
    }

    // BUG:在冲刺过程中，两个尘埃特效会同时触发，type疑似失效
    public void CreateSprintDust(Transform playerTran, float dir, ParticalEffectType type)
    {
        var objPool = type switch
        {
            ParticalEffectType.SprintDust => poolEffectList[0],
            ParticalEffectType.UnderDust => poolEffectList[1],
            _ => null
        };
        GameObject obj = objPool.Get();
        obj.transform.position = playerTran.position;
        obj.transform.localScale = new Vector3(dir, 1, 1);
    }
}
