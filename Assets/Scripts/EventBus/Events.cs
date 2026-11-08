using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IEvent
{
}

#region =========场景相关=========

public struct SceneLoadEvent : IEvent
{
    public GameSceneSO LocationToLoad;
    public Vector3 posToGo;
    public bool fadeScreen;
}

public struct SceneUnloadEvent : IEvent
{
}

public struct BackToMenuEvent : IEvent
{
}

public struct AfterSceneLoadEvent : IEvent
{
    public GameSceneSO LoadScene;
}

public struct SceneFadeEvent : IEvent
{
    public Color target;
    public float Duration;
    public bool fadeIn;
}

#endregion

#region =========玩家相关=========

public struct FocusChangeEvent : IEvent
{
    public bool isGetFous;
    public float changeFocusCount;
}

public struct FocusUIChangeEvent : IEvent
{
    public float currentFocus;
    public float maxFocus;
}

public struct PoseChangeEvent : IEvent
{
    public Character character;
    public float targetPosture;
    public float maxPosture;
    public float duration;
}

// public struct HurtEvent : IEvent
// {
//     public Character character;
// }
//
// public struct DeathEvent : IEvent
// {
//     public Character character;
// }

#endregion

#region =========音频与特效=========

public struct FxEvent : IEvent
{
    public Vector3 playerTran;
    public float dir;
    public ParticalEffectType type;
}

public struct ReliseFxEvent : IEvent
{
}

public struct SynuEvent : IEvent
{
    public float count;
}

#endregion

#region =========相机震动=========

public struct BlockCameraShakeEvent : IEvent
{
    public float Force;
}

public struct HeavyAtkCameraShakeEvent : IEvent
{
    public float Force;
}

#endregion


/// <summary>
/// 疑似本地化事件
/// </summary>
public struct LoadDataEvent : IEvent
{
}

#region =========游戏始末相关=========

public struct NewGameEvent : IEvent
{
}

public struct GameOverEvent : IEvent
{
}

public struct PauseEvnet : IEvent
{
}

#endregion