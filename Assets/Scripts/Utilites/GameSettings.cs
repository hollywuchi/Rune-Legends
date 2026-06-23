using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    void Awake()
    {
        // 尽可能使用高的刷新率
        Application.targetFrameRate = 120;
    }
}
