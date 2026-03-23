using System.Collections.Generic;
using UnityEngine;

public class Data
{
    public string sceneToSave;
    public Dictionary<string, Vector3> characterPosDic = new Dictionary<string, Vector3>();
    public Dictionary<string, float> characterHealth = new Dictionary<string, float>();

    #region 工厂模式 产生的转化JSON方法
    public void SaveGameScene(GameSceneSO gameSceneSO)
    {
        sceneToSave = JsonUtility.ToJson(gameSceneSO);  //可以将任何object类型的文件（class,scriptobject）转化为json
    }
    public GameSceneSO GetSaveScene()
    {
        var newScene = ScriptableObject.CreateInstance<GameSceneSO>();
        JsonUtility.FromJsonOverwrite(sceneToSave, newScene);
        Debug.Log("JSON已转化");
        return newScene;
    }
    #endregion
}
// TODO:工厂模式
// 保存了人物的血量，和场景信息