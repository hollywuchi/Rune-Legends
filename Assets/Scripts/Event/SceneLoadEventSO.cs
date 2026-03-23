using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/SceneLoadEventSO")]
public class SceneLoadEventSO : ScriptableObject
{
    public UnityAction<GameSceneSO,Vector3,bool> LoadRequestEvent;

    /// <summary>
    /// 场景加载需求
    /// </summary>
    /// <param name="LocationToLoad">要加载的地图</param>
    /// <param name="posToGo">player落地的坐标</param>
    /// <param name="fadeScreen">是否需要渐入渐出过度</param>
    public void RaiseLoadRequestEvent(GameSceneSO LocationToLoad,Vector3 posToGo,bool fadeScreen)
    {
        LoadRequestEvent?.Invoke(LocationToLoad,posToGo,fadeScreen);
    }
}