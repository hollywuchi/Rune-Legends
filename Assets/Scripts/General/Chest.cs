using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    AudioDefination audioDefination;
    public Sprite openSprite;
    public Sprite closeSprite; 
    public bool isDone;
    SpriteRenderer NowSprite;
    private void Awake() 
    {
        NowSprite = GetComponent<SpriteRenderer>();
        audioDefination = GetComponent<AudioDefination>();
    }
    private void OnEnable() 
    {
        NowSprite.sprite = isDone?openSprite:closeSprite;
    }
    public void TriggerAction()
    {
        if(!isDone)
        {
            OpenChest();
        }
    }
    public void OpenChest()
    {
        // 打开宝箱之后运行的代码
        NowSprite.sprite = openSprite;
        audioDefination.PlayAudioClip();
        isDone = true;
        gameObject.tag = "Untagged";
    }
}
