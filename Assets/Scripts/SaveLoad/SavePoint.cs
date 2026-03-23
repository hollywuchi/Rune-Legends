using UnityEngine;

public class SavePoint : MonoBehaviour, IInteractable
{
    [Header("广播")]
    public VoidSo SaveDataEvent;
    [Header("参数设置")]
    public SpriteRenderer NowSprite;
    public Sprite lightSprite;
    public Sprite darkSprite;
    public bool isDone;
    public GameObject Light;

    private void OnEnable()
    {
        NowSprite.sprite = isDone ? lightSprite : darkSprite;
        Light.SetActive(isDone);
    }

    public void TriggerAction()
    {
        if (!isDone)
        {
            NowSprite.sprite = lightSprite;
            isDone = true;
            // 保存数据
            SaveDataEvent.RaiseEvent();
            Light.SetActive(true);
        }
    }

}
