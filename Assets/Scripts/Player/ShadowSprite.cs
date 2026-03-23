using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowSprite : MonoBehaviour
{
    Transform Player;
    SpriteRenderer thisSprite;
    SpriteRenderer playerSprite;
    Color _color;
    [Header("时间控制参数")]
    public float activeTime;
    float activeStart;
    [Header("不透明度控制")]
    float alpha;//当前控制值
    public float alphaSet;//初始值
    public float alphaMultiplier;//乘以一个固定的值来表示逐渐消失的效果
    private void OnEnable() 
    {
        // 获得私有的组件
        Player = GameObject.FindWithTag("Player").transform;
        thisSprite = GetComponent<SpriteRenderer>();
        playerSprite = Player.GetComponent<SpriteRenderer>();

        // 初始化所有的私有参数值
        alpha = alphaSet;
        thisSprite.sprite = playerSprite.sprite;

        transform.position = Player.position;
        transform.localScale = Player.localScale;
        
        // 截取冲刺开始的时间点
        activeStart = Time.time;
    }
    void Update()
    {
        // 在update的每一帧执行的过程中，我们要逐渐增加残影的透明度
        // 并且控制时间，如果时间超过我们想要的时间，那么那个残影就要发配回到对象池中

        alpha -= alphaMultiplier;

        _color = new Color(1,1,0,alpha);

        thisSprite.color = _color;

        if(Time.time >= activeStart + activeTime)
        {
            // 遣返回对象池
            ShadowPool.instance.ReturenPool(gameObject);
        }

        
    }
}
