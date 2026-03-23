using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Game Scene/GameSceneSO")]
public class GameSceneSO : ScriptableObject
{
    public SceneType sceneType;
    // 场景的引用
    public AssetReference sceneReference;
}
