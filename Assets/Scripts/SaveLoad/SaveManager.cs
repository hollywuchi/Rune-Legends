using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    Data saveData;
    public List<ISaveable> saveableList = new List<ISaveable>();           //要保存的场景中的东西
    [Header("事件监听")]
    public VoidSo SaveDataEvent;
    public VoidSo LoadDataEvent;
    void Awake()    //实现单例模式，初始化存储单元
    {
        if (instance == null)               //单例模式的实现
        {                                   //但是仅仅是为了保存数据使用的
            instance = this;                //本游戏使用的是观察者模式
        }
        else
            Destroy(this.gameObject);

        saveData = new Data();
    }
    void OnEnable()
    {
        SaveDataEvent.OnEventRaised += Save;
        LoadDataEvent.OnEventRaised += Load;
    }
    void OnDisable()
    {
        SaveDataEvent.OnEventRaised -= Save;
        LoadDataEvent.OnEventRaised -= Load;
    }

    void Update()
    {
        // print(saveableList.Count);
        if(Keyboard.current.lKey.wasPressedThisFrame)
        {
            Load();
        }
        if(Keyboard.current.kKey.wasPressedThisFrame)
        {
            Save();
        }
    }
    public void RegisterSaveData(ISaveable saveable) //注册方法
    {
        if (!saveableList.Contains(saveable))
        {
            saveableList.Add(saveable);     //如果没有，就要加上
            // Debug.Log(saveable + "已加入");
            // Debug.Log("现在共有" + saveableList.Count + "项");
        }
    }
    public void UnRegisterSaveData(ISaveable saveable) //注销方法（实际上就是整个注销）
    {
        saveableList.Remove(saveable);
        // Debug.Log(saveable + "已注销");
    }
    public void Save()  //整个保存的主方法（向所有继承接口的部分广播）
    {
        foreach (var saveable in saveableList)
        {
            saveable.GetSaveData(saveData);     //将现有的空的数据传递到要保存的位置填充内容
        }

        //仅供展示作用
        foreach (var item in saveData.characterPosDic)
        {
            Debug.Log(item.Key + "   " + item.Value);
        }
        foreach(var item in saveData.characterHealth)
        {
            Debug.Log(item.Key + "  " + item.Value);
        }
        Debug.Log(saveData.sceneToSave);
        
    }
    public void Load()  //整个加载的主方法（向所有继承接口的部分广播）
    {
        foreach (var saveable in saveableList)
        {
            saveable.loadData(saveData);
        }
        // Debug.Log("已加载" + saveableList.Count + "项");
    }
}

