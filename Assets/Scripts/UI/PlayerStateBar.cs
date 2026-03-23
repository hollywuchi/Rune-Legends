using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateBar : MonoBehaviour
{
   public Image HealthImage;
   public Image DelayHealthImage;
   public Image PowerImage;
//    public SceneLoadEventSO SceneUnloadEvent;

    void OnEnable()
    {
        // SceneUnloadEvent.
    }

    void OnDisable()
    {
        
    }
    private void Update() 
    {
        if(HealthImage.fillAmount < DelayHealthImage.fillAmount)
        {
            DelayHealthImage.fillAmount -= Time.deltaTime * 0.15f;
        }
    }
    /// <summary>
    /// 血量减少时会改变图片长度
    /// </summary>
    /// <param name="persent">百分比的量</param>
    public void ChangeHealth(float percent)
    {
        HealthImage.fillAmount = percent;
    }

}
