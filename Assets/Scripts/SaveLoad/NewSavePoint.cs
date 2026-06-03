using System.Collections;
using UnityEngine;

public class NewSavePoint : MonoBehaviour
{
    public SpriteRenderer lightSprite;
    public Animator anim;

    public bool isActivated = false;

    // FIXME：-- 玩家激活动画完成之后无法退出
    // FIXME：-- 玩家激活动画之后，亮起的标志无法持续
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerContext context = collision.GetComponent<Player>().ctx;
            if (context == null) return;

            if (!context.IsActivating && context.ActivatePressedThisFrame)
            {
                print("准备激活");
                anim.SetBool("IsActivating", true);
                StartCoroutine(ChangeColor(3f)); // 1秒内变为透明
                context.IsActivating = true;
                isActivated = true;
            }

            else if (!context.IsHoldingActivate)
            {
                anim.SetBool("IsActivating", false);
                lightSprite.color = new Color(1, 1, 1, 0); // 恢复原色
                context.IsActivating = false;
                isActivated = false;
            }

        }
    }

    private IEnumerator ChangeColor(float duration)
    {
        float elapsed = 0f;
        Color initialColor = lightSprite.color;
        Color targetColor = new Color(1, 1, 1, 1);

        while (elapsed < duration)
        {
            lightSprite.color = Color.Lerp(initialColor, targetColor, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        lightSprite.color = targetColor;
    }
}
