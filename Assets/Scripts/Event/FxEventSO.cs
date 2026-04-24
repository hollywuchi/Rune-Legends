using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(menuName = "Event/FxEventSO")]
public class FxEventSO : ScriptableObject
{
    public UnityAction<Transform, float, ParticalEffectType> FxSpawnEvent;
    public void RaiseFxEvent(Transform trans, float dir, ParticalEffectType type)
    {
        // 这里可以添加一些全局的特效管理逻辑，比如根据类型选择不同的池子，或者添加一些全局的特效设置
        FxSpawnEvent?.Invoke(trans, dir, type);
    }

}