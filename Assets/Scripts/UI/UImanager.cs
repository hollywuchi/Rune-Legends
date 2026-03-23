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
    public VoidSo GameOverEvent;
    public VoidSo BackToMenuEvent;
    public FloatEventSO SynuEvent;
    [Header("事件广播")]
    public VoidSo PauseEvnet;
    [Header("组件")]
    public GameObject GameOverPanle;
    public GameObject RestartBut;
    public GameObject mobleTouch;
    public GameObject PauseMenu;
    public Button SettingButton;
    public Slider AudioSlider;

    void Awake()
    {
#if UNITY_STANDALONE    //此时代表的是主机模式下
        mobleTouch.SetActive(false);    //默认不启动控制面板
#endif
        SettingButton.onClick.AddListener(TogglePausePanle);    //添加一个监听
    }
    private void OnEnable()
    {
        HealthEvent.OnEvenetRaised += HealthChange;
        SceneLoadEvent.LoadRequestEvent += checkMenu;
        LoadDataEvent.OnEventRaised += LoadData;
        GameOverEvent.OnEventRaised += GameOver;
        BackToMenuEvent.OnEventRaised += LoadData;
        SynuEvent.OnEventRaised += OnSync;
    }



    private void OnDisable()
    {
        HealthEvent.OnEvenetRaised -= HealthChange;
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
            PauseMenu.SetActive(false);
            Time.timeScale = 1;             //开始游戏
        }
        else
        {
            PauseEvnet.RaiseEvent();
            PauseMenu.SetActive(true);
            Time.timeScale = 0;             //暂停游戏
        }
    }
    private void GameOver()
    {
        GameOverPanle.SetActive(true);
        EventSystem.current.SetSelectedGameObject(RestartBut);
    }

    private void LoadData()
    {
        GameOverPanle.SetActive(false);
    }

    private void checkMenu(GameSceneSO SceneLoaded, Vector3 arg1, bool arg2)
    {
        isMenu = SceneLoaded.sceneType == SceneType.Menu;
        bar.gameObject.SetActive(!isMenu);
    }


    private void HealthChange(Character character)
    {
        var persent = character.CurrentHealth / character.maxHealth;
        bar.ChangeHealth(persent);
    }
}
