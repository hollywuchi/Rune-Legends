using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
    public GameObject NewGameButton;

    private void OnEnable() 
    {
        // 默认选中第一个按钮
        EventSystem.current.SetSelectedGameObject(NewGameButton);    
    }
    public void ExitGame()
    {
        Debug.Log("退出游戏");
        Application.Quit();
    }
}
