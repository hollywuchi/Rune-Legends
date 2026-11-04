using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UImanager : MonoBehaviour
{
    public bool isMenu;
    public PlayerStateBar bar;
    [Header("事件监听")]
    // public CharacterEventSo HealthEvent;
    private EventBinding<HealthChangeEvent> characterHealthEventBinding;
    private EventBinding<FocusChangeEvent> focusChangeEventBinding;
    private EventBinding<SceneLoadEvent> sceneLoadEventBinding;
    private EventBinding<LoadDataEvent> loadDataEventBinding;
    private EventBinding<AfterSceneLoadEvent> AfterLoadSceneEventBinding;
    private EventBinding<GameOverEvent> gameOverEventBinding;
    private EventBinding<BackToMenuEvent> backToMenuEventBinding;
    private EventBinding<SynuEvent> synuEventBinding;
    [Header("组件")]
    public UIAnimations UIanim;
    public GameObject PoseUI;
    public GameObject GameOverPanle;
    public GameObject RestartBut;
    public GameObject mobleTouch;
    public GameObject PauseMenu;
    public Button SettingButton;
    public Button PauseMenuSettingButton;
    public Slider AudioSlider;
    public InputManager Actions;

    void Awake()
    {
        SettingButton.onClick.AddListener(TogglePausePanle);    //添加一个监听
    }
    private void OnEnable()
    {
        characterHealthEventBinding = new EventBinding<HealthChangeEvent>(HealthChange);
        focusChangeEventBinding = new EventBinding<FocusChangeEvent>(PowerChange);
        backToMenuEventBinding = new EventBinding<BackToMenuEvent>(LoadData);
        loadDataEventBinding = new EventBinding<LoadDataEvent>(LoadData);
        AfterLoadSceneEventBinding = new EventBinding<AfterSceneLoadEvent>(CheckPlatformAndMenu);
        gameOverEventBinding = new EventBinding<GameOverEvent>(GameOver);
        synuEventBinding = new EventBinding<SynuEvent>(OnSync);


        EventBus<HealthChangeEvent>.Register(characterHealthEventBinding);
        EventBus<FocusChangeEvent>.Register(focusChangeEventBinding);
        EventBus<AfterSceneLoadEvent>.Register(AfterLoadSceneEventBinding);
        EventBus<LoadDataEvent>.Register(loadDataEventBinding);
        EventBus<GameOverEvent>.Register(gameOverEventBinding);
        EventBus<BackToMenuEvent>.Register(backToMenuEventBinding);
        EventBus<SynuEvent>.Register(synuEventBinding);
    }

    void Start()
    {
        Actions = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().inputActions;   //获取输入系统
        Actions.UI.Pause.performed += _ => TogglePausePanle();   //添加一个监听，按下暂停键时调用TogglePausePanle方法
    }



    private void OnDisable()
    {
        EventBus<HealthChangeEvent>.Deregister(characterHealthEventBinding);
        EventBus<FocusChangeEvent>.Deregister(focusChangeEventBinding);
        EventBus<AfterSceneLoadEvent>.Deregister(AfterLoadSceneEventBinding);
        EventBus<LoadDataEvent>.Deregister(loadDataEventBinding);
        EventBus<GameOverEvent>.Deregister(gameOverEventBinding);
        EventBus<BackToMenuEvent>.Deregister(backToMenuEventBinding);
        EventBus<SynuEvent>.Deregister(synuEventBinding);
    }

    private void OnSync(SynuEvent e)
    {
        AudioSlider.value = e.count;
    }

    public void TogglePausePanle()
    {
        if (PauseMenu.activeInHierarchy)     //*activeInHierarchy判断是否为开启状态,可以做到简单的逻辑门
        {
            UIanim.PlayExitAnimation(() =>
            {
                PauseMenu.SetActive(false);
                Time.timeScale = 1;
            });    //播放退出动画，动画结束后关闭菜单
        }
        else
        {
            EventBus<PauseEvnet>.Raise(new PauseEvnet { });
            PauseMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(PauseMenuSettingButton.gameObject);   //设置默认选中按钮
            Time.timeScale = 0;             //暂停游戏
        }
    }
    private void GameOver()
    {
        GameOverPanle.SetActive(true);
        EventSystem.current.SetSelectedGameObject(RestartBut);
    }
    public void CheckPlatformAndMenu(AfterSceneLoadEvent e)
    {
        mobleTouch.SetActive(Application.isMobilePlatform);

        isMenu = e.LoadScene.sceneType == SceneType.Menu;
        bar.gameObject.SetActive(!isMenu);
        SettingButton.gameObject.SetActive(!isMenu);
    }

    private void LoadData()
    {
        GameOverPanle.SetActive(false);
        mobleTouch.SetActive(false);
    }

    private void HealthChange(HealthChangeEvent e)
    {
        if (!e.character.CompareTag("Player")) return;
        var persent = e.character.CurrentHealth / e.character.maxHealth;
        bar.ChangeHealth(persent);
    }

    private void PowerChange(FocusChangeEvent e)
    {
        if (!e.character.CompareTag("Player")) return;
        var persent = e.character.currentFocus / e.character.config.maxFocus;
        bar.ChangePower(persent);
    }

    public void OpenLink(string url)
    {
        Application.OpenURL(url);
    }

    public void BackToMenu()
    {
        EventBus<BackToMenuEvent>.Raise(new BackToMenuEvent { });
    }
}
