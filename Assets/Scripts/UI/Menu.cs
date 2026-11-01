using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject NewGameButton;
    public Button startButton;

    private void OnEnable()
    {
        // 默认选中第一个按钮
        EventSystem.current.SetSelectedGameObject(NewGameButton);
        startButton.onClick.AddListener(StartNewGame);
    }
    void OnDisable()
    {
        startButton.onClick.RemoveAllListeners();
    }
    private void StartNewGame()
    {
        EventBus<NewGameEvent>.Raise(new NewGameEvent { });
    }


    public void ExitGame()
    {
        Debug.Log("退出游戏");
        Application.Quit();
    }
}
