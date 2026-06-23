using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UImanager : MonoBehaviour
{
    public bool isMenu;
    public PlayerStateBar bar;
    [Header("事件监听")]
    public CharacterEventSo HealthEvent;
    public SceneLoadEventSO SceneLoadEvent;
    public VoidSo LoadDataEvent;
    public VoidSo afterLoadSceneEvent;
    public VoidSo GameOverEvent;
    public VoidSo BackToMenuEvent;
    public FloatEventSO SynuEvent;
    [Header("事件广播")]
    public VoidSo PauseEvnet;
    [Header("组件")]
    public UIAnimations UIanim;
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

    // FIXME:玩家在重生之后没有UI，现在的摇杆手感还是稀烂
    private void OnEnable()
    {
        afterLoadSceneEvent.OnEventRaised += PlatformCheck;
        HealthEvent.OnEvenetRaised += HealthChange;
        HealthEvent.OnEvenetRaised += PowerChange;
        SceneLoadEvent.LoadRequestEvent += checkMenu;
        LoadDataEvent.OnEventRaised += LoadData;
        GameOverEvent.OnEventRaised += GameOver;
        BackToMenuEvent.OnEventRaised += LoadData;
        SynuEvent.OnEventRaised += OnSync;
    }

    void Start()
    {
        Actions = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().inputActions;   //获取输入系统
        Actions.UI.Pause.performed += _ => TogglePausePanle();   //添加一个监听，按下暂停键时调用TogglePausePanle方法
    }

    

    private void OnDisable()
    {
        afterLoadSceneEvent.OnEventRaised -= PlatformCheck;
        HealthEvent.OnEvenetRaised -= HealthChange;
        HealthEvent.OnEvenetRaised -= PowerChange;
        SceneLoadEvent.LoadRequestEvent -= checkMenu;
        LoadDataEvent.OnEventRaised -= LoadData;
        GameOverEvent.OnEventRaised -= GameOver;
        BackToMenuEvent.OnEventRaised -= LoadData;
        SynuEvent.OnEventRaised -= OnSync;
    }

    private void OnSync(float Count)
    {
        AudioSlider.value = Count;
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
            PauseEvnet.RaiseEvent();
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
    private void PlatformCheck()
    {
        mobleTouch.SetActive(Application.isMobilePlatform);
    }

    private void LoadData()
    {
        GameOverPanle.SetActive(false);
        mobleTouch.SetActive(false);
    }

    private void checkMenu(GameSceneSO SceneLoaded, Vector3 arg1, bool arg2)
    {
        isMenu = SceneLoaded.sceneType == SceneType.Menu;
        bar.gameObject.SetActive(!isMenu);
        SettingButton.gameObject.SetActive(!isMenu);
    }


    private void HealthChange(Character character)
    {
        var persent = character.CurrentHealth / character.maxHealth;
        bar.ChangeHealth(persent);
    }

    private void PowerChange(Character character)
    {
        var persent = character.currentFocus / character.config.maxFocus;
        bar.ChangePower(persent);
    }

    public void OpenLink(string url)
    {
        Application.OpenURL(url);
    }
}
