using System.Collections;
using UnityEngine;
using DG.Tweening;

public class NewSavePoint : MonoBehaviour
{
    public SpriteRenderer lightSprite;
    public Animator anim;
    public AnimationClip animClip;

    Vector3 pos;
    Vector3 targetPos;

    public bool isActivated;

    void Awake()
    {
        pos = lightSprite.transform.position;
        targetPos = pos + new Vector3(0, 0.3f, 0);
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerContext context = collision.GetComponent<Player>().ctx;
            if (context == null) return;

            if (!isActivated && !DOTween.IsTweening(lightSprite) && context.IsHoldingActivate)
            {
                anim.SetBool("IsActivating", true);

                // 1. 颜色线性渐变（SetEase(Ease.Linear) 确保绝对线性）
                lightSprite.DOColor(new Color(1, 1, 1, 1), animClip.length * 0.9f).SetEase(Ease.Linear);

                // 2. 位置线性移动，并在结束时触发回调
                lightSprite.transform.DOMove(pos + new Vector3(0, 0.3f, 0), animClip.length * 0.9f)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        anim.SetBool("IsActivating", false);
                        isActivated = true;
                    });
            }

            // 玩家松开按键的打断逻辑
            if (DOTween.IsTweening(lightSprite) && !context.IsHoldingActivate)
            {
                lightSprite.DOKill(); // 杀死动画
                lightSprite.color = new Color(1, 1, 1, 0);
                lightSprite.transform.position = pos;
                anim.SetBool("IsActivating", false);
                isActivated = false;
            }
            context.CanRest = isActivated;
        }

    }
}

