using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;

public interface ISaveable
{
    DataDefination GetSaveID();
    void RegisterSaveData() => SaveManager.instance.RegisterSaveData(this);     //记录数据，初始化的作用
    // void RegisterSaveData()
    // {
    //     SaveManager.instance.RegisterSaveData(this);
    // }
    void UnRegisterSaveData() => SaveManager.instance.UnRegisterSaveData(this);   //注销数据，方法体也是初始化的作用
    void GetSaveData(Data data);          //获取储存的数据
    void loadData(Data data);             //加载数据

}
