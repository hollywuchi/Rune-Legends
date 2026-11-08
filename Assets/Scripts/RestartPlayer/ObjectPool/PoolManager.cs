using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    [Header("特效对象列表")]
    public List<GameObject> poolPrefabs;
    [Header("事件监听")]
    private EventBinding<FxEvent> fxEventBing;
    // 对象池的列表
    private List<ObjectPool<GameObject>> poolEffectList = new List<ObjectPool<GameObject>>();
    // private Queue<GameObject> soundQueue = new Queue<GameObject>();     // 队列，当做音效的对象池

    void Awake()
    {
        CreatePool();
    }
    void OnEnable()
    {
        fxEventBing = new EventBinding<FxEvent>(CreateFX);
        
        EventBus<FxEvent>.Register(fxEventBing);
    }

    void OnDisable()
    {
        EventBus<FxEvent>.Deregister(fxEventBing);
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
    private void CreateFX(FxEvent e)
    {
        var objPool = e.type switch
        {
            ParticalEffectType.SprintDust => poolEffectList[0],
            ParticalEffectType.UnderDust => poolEffectList[1],
            ParticalEffectType.AirDust => poolEffectList[2],
            ParticalEffectType.Hit => poolEffectList[3],
            ParticalEffectType.Charging => poolEffectList[4],
            ParticalEffectType.BlockFx => poolEffectList[5],
            ParticalEffectType.PerfBlock => poolEffectList[6],
            ParticalEffectType.WaveFX => poolEffectList[7],
            _ => null
        };
        if (objPool == null) return;
        
        GameObject obj = objPool.Get();
        obj.transform.position = e.playerTran;
        obj.transform.localScale = new Vector3(e.dir, obj.transform.localScale.y, obj.transform.localScale.z);
    }
}
