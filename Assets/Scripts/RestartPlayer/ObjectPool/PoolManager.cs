using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
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

            ObjectPool<GameObject> pool = null;

            pool = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    GameObject obj = Instantiate(item, parent);
                    obj.GetComponent<Effect_Elements>().SetPool(pool);
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

    // 在冲刺过程中，两个尘埃特效会同时触发，type疑似失效
    // 因为两个对象池相互污染，因为不是临时变量，所以调用的时候，会默认调用最后一个使用的对象池，然后就污染了
    public void CreateFX(Transform playerTran, float dir, ParticalEffectType type)
    {
        var objPool = type switch
        {
            ParticalEffectType.SprintDust => poolEffectList[0],
            ParticalEffectType.UnderDust => poolEffectList[1],
            ParticalEffectType.AirDust => poolEffectList[2],
            _ => null
        };
        GameObject obj = objPool.Get();
        obj.transform.position = playerTran.position;
        obj.transform.localScale = new Vector3(dir, 1, 1);
    }
}
