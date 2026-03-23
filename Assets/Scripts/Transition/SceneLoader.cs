using System;
using System.Collections;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour, ISaveable
{
    #region 参数部分
    public Transform playerPos;
    public Vector3 firstPosition;
    public Vector3 menuPosition;
    [Header("事件监听")]
    public SceneLoadEventSO SceneLoadEventSO;
    public VoidSo NewGameEvent;
    public VoidSo BackToMenuEvent;
    [Header("事件广播")]
    public VoidSo SceneAfterEvnet;
    public FadeEventSO fadeEvent;
    public SceneLoadEventSO SceneUnloadEvent;
    [Header("场景")]
    public GameSceneSO firstLoadScene;
    public GameSceneSO menuScene;
    GameSceneSO currentLoadedScene;
    GameSceneSO SceneToLoad;
    Vector3 Newplayerpos;
    bool FadeScreen;
    bool isLoading;

    public float fadeduration; // 等待的时间
    #endregion
    private void Awake()
    {
        // // Addressables.LoadSceneAsync(firstLoadScene.sceneReference,LoadSceneMode.Additive);
        // currentLoadedScene  = firstLoadScene;
        // // 异步加载场景，加载场景的代码
        // currentLoadedScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive);

    }
    private void Start()
    {
        // Initialization();    
        SceneLoadEventSO.RaiseLoadRequestEvent(menuScene, menuPosition, true);
        ISaveable saveable = this;
        saveable.RegisterSaveData();
    }
    private void OnEnable()
    {

        SceneLoadEventSO.LoadRequestEvent += OnLoadRequestEvent;
        NewGameEvent.OnEventRaised += Initialization;
        BackToMenuEvent.OnEventRaised += BackToMenu;

    }
    private void OnDisable()
    {
        SceneLoadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
        NewGameEvent.OnEventRaised -= Initialization;
        BackToMenuEvent.OnEventRaised -= BackToMenu;

        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }



    /// <summary>
    /// 初始化场景
    /// </summary>
    public void Initialization()
    {
        SceneToLoad = firstLoadScene;
        // OnLoadRequestEvent(SceneToLoad,firstPosition,true);
        SceneLoadEventSO.RaiseLoadRequestEvent(SceneToLoad, firstPosition, true);
    }
    private void OnLoadRequestEvent(GameSceneSO sceneToLoad, Vector3 posToGo, bool fadeScreen)
    {
        if (isLoading)
            return;
        isLoading = true;
        SceneToLoad = sceneToLoad;
        Newplayerpos = posToGo;
        FadeScreen = fadeScreen;

        // 如果当前没有场景，那么就加载一个场景进来
        if (currentLoadedScene != null)
        {
            StartCoroutine(UnLoadPreviousScene());
        }
        else
        {
            loadNewScene();
        }
    }

    /// <summary>
    /// 卸载之前的场景
    /// </summary>
    /// <returns></returns>
    IEnumerator UnLoadPreviousScene()
    {
        if (FadeScreen)
        {
            fadeEvent.FadeIn(fadeduration);
        }
        yield return new WaitForSeconds(fadeduration);
        // 广播关闭血条显示(与之前逻辑不同，这次是在卸载时处理的UI，之前是在卸载之前处理的)
        SceneUnloadEvent.RaiseLoadRequestEvent(SceneToLoad, Newplayerpos, true);
        yield return currentLoadedScene.sceneReference.UnLoadScene();
        // 关闭人物防止下坠
        playerPos.gameObject.SetActive(false);
        loadNewScene();
    }
    /// <summary>
    /// 加载新的场景
    /// </summary>
    void loadNewScene()
    {
        var loadingOption = SceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        loadingOption.Completed += OnloadCompleted;// 场景加载事件，加载完成之后，这是写好的方法
    }

    private void OnloadCompleted(AsyncOperationHandle<SceneInstance> obj)
    {
        currentLoadedScene = SceneToLoad;//改变当前场景（理论上）
        playerPos.position = Newplayerpos;//传送玩家
        playerPos.gameObject.SetActive(true);//玩家可视化
        // 再次强制注册玩家
        ISaveable saveable = playerPos.gameObject.GetComponent<Character>();
        saveable.RegisterSaveData();
        if (FadeScreen)
        {
            fadeEvent.FadeOut(fadeduration);//淡出界面
        }
        isLoading = false;

        if (currentLoadedScene.sceneType == SceneType.loaction)
            SceneAfterEvnet.RaiseEvent();//场景加载完成之后将事件触发广播出去
    }

    public void BackToMenu()
    {
        SceneToLoad = menuScene;
        SceneLoadEventSO.RaiseLoadRequestEvent(SceneToLoad, menuPosition, true);
    }
    #region 数据保存
    public DataDefination GetSaveID()
    {
        return GetComponent<DataDefination>();
    }

    public void GetSaveData(Data data)
    {
        data.SaveGameScene(currentLoadedScene);
    }

    public void loadData(Data data)
    {
        var playerID = playerPos.GetComponent<DataDefination>().ID;
        if (data.characterPosDic.ContainsKey(playerID))
        {
            SceneToLoad = data.GetSaveScene();
            var posToGo = data.characterPosDic[playerID];

            SceneLoadEventSO.RaiseLoadRequestEvent(SceneToLoad, posToGo, true);
        }
    }
    #endregion
}
