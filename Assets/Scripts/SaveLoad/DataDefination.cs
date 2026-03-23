using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 这个脚本是用来生成一个固定的GUID，方便使用字典来保存数据的
// GUID是一个16位的串码，可以代替名称来实现一对一的查找，类似哈希查找
public class DataDefination : MonoBehaviour
{
    public PersistentType persistentType;
    public string ID;               //将生成的ID显示出来

    void OnValidate()
    {
        if (persistentType == PersistentType.readwrite)
        {
            if (ID == string.Empty)
            {
                ID = System.Guid.NewGuid().ToString(); //mono为我们写好的方法，可以直接生成一个随机的GUID
            }
        }
    }
}
