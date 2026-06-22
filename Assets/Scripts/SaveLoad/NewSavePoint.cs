using System.Collections;
using UnityEngine;
using DG.Tweening;

public class NewSavePoint : MonoBehaviour
{
    public SpriteRenderer lightSprite;
    public Animator anim;
    public AnimationClip animClip;

    private Vector3 pos;
    private Vector3 targetPos;
    private Sequence activateSeq;

    public Vector3 ResurrectPoint;  // 复活点位置
    private PlayerContext context;
    public bool isActivated;

    void Awake()
    {
        pos = lightSprite.transform.position;
        targetPos = pos + new Vector3(0, 0.3f, 0);
        ResurrectPoint = GameObject.FindGameObjectWithTag("ResurrectPoint").transform.position;
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            context = collision.GetComponent<Player>().ctx;
            if (context == null) return;

            // 判断当前 Sequence 是否存在且正在播放
            bool isTweening = activateSeq != null && activateSeq.IsActive();

            if (!isActivated && !isTweening && context.IsHoldingActivate)
            {
                anim = collision.GetComponent<Animator>();
                anim.SetBool("IsActivating", true);

                float duration = animClip.length * 0.9f;

                // 创建一个新的 Sequence 让两个动画同时进行，更安全高效
                activateSeq = DOTween.Sequence();

                // Join 表示与上一个动画同时执行
                activateSeq.Join(lightSprite.DOColor(new Color(1, 1, 1, 1), duration).SetEase(Ease.Linear));
                activateSeq.Join(lightSprite.transform.DOMove(pos + new Vector3(0, 0.3f, 0), duration).SetEase(Ease.Linear));

                // 整个序列完成时的回调
                activateSeq.OnComplete(() =>
                {
                    anim.SetBool("IsActivating", false);
                    isActivated = true;
                    context.ResurrectPoint = ResurrectPoint;
                });
            }

            // 玩家松开按键的打断逻辑
            if (isTweening && !context.IsHoldingActivate)
            {
                activateSeq.Kill();
                activateSeq = null;

                lightSprite.color = new Color(1, 1, 1, 0);
                lightSprite.transform.position = pos;
                anim.SetBool("IsActivating", false);
                isActivated = false;
            }

            context.CanRest = isActivated;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (context == null) return;
            context.CanRest = false;
        }
    }
}

