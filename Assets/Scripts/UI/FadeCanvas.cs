using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class FadeCanvas : MonoBehaviour
{
    public Image FadeImage;
    [Header("事件监听")]
    public FadeEventSO fadeEvent;
    private void OnEnable()
    {
        fadeEvent.OnEventRaised += OnFadeEvent;
    }
    private void OnDisable() 
    {
        fadeEvent.OnEventRaised -= OnFadeEvent;
    }
    void OnFadeEvent(Color target,float Duration,bool fadeIn)
    {
        FadeImage.DOBlendableColor(target,Duration);
    }
}
