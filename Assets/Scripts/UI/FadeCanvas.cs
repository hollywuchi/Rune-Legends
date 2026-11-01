using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class FadeCanvas : MonoBehaviour
{
    public Image FadeImage;
    // [Header("事件监听")]
    // public FadeEventSO fadeEvent;
    private EventBinding<SceneFadeEvent> sceneFadeEventBinding;
    private void OnEnable()
    {
        sceneFadeEventBinding = new EventBinding<SceneFadeEvent>(OnFadeEvent);
        
        EventBus<SceneFadeEvent>.Register(sceneFadeEventBinding);
    }
    private void OnDisable() 
    {
        EventBus<SceneFadeEvent>.Deregister(sceneFadeEventBinding);
    }
    void OnFadeEvent(SceneFadeEvent e)
    {
        FadeImage.DOBlendableColor(e.target,e.Duration);
    }
}
