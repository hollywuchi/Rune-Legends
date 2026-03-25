using UnityEngine;

// 淡入淡出和背景区域化
public class SpriteGroupAlphaController : MonoBehaviour
{
    private SpriteRenderer[] renders;
    [SerializeField] private float currentAlpha;

    [Header("玩家组件")]
    public Transform playerTrans;

    [Header("淡入淡出的速度")]
    public float fadeOutSpeed = 1f;
    public float fadeInSpeed = 1f;

    [SerializeField] private float currentTargetAlpha;
    [SerializeField] private bool playerInside = false;
    private float lastPlayerPosX;

    [Header("判断玩家到底进入了那个区域")]
    public AreaName areaName;
    public AreaName currentArea;
    void Awake()
    {
        renders = GetComponentsInChildren<SpriteRenderer>();

        if (currentArea != areaName && playerInside == false)
        {
            currentTargetAlpha = 0;
            SetAlpha(currentTargetAlpha);
            // Debug.Log("设置alpha值为" + currentTargetAlpha);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerTrans = collision.transform;
            playerInside = true;
            lastPlayerPosX = playerTrans.position.x;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInside = false;
        }
    }

    void Update()
    {
        if (playerInside && playerTrans != null)
        {
            DetectPlayerMove();
        }

        SetAlpha(currentTargetAlpha);
    }

    /// <summary>
    /// 判断玩家移动方向
    /// </summary>
    private void DetectPlayerMove()
    {
        float movementDir = playerTrans.position.x - lastPlayerPosX;

        if (areaName == AreaName.Forest)
        {
            if (movementDir >= 0.001f)
                currentTargetAlpha -= fadeOutSpeed * Time.deltaTime;
            else if (movementDir <= -0.001f)
                currentTargetAlpha += fadeInSpeed * Time.deltaTime;
        }

        if (areaName == AreaName.Mountain)
        {
            if (movementDir >= 0.001f)
                currentTargetAlpha += fadeInSpeed * Time.deltaTime;
            else if (movementDir <= -0.001f)
                currentTargetAlpha -= fadeOutSpeed * Time.deltaTime;
        }

        lastPlayerPosX = playerTrans.position.x;
        currentTargetAlpha = Mathf.Clamp01(currentTargetAlpha);
    }

    /// <summary>
    /// 设置当前Alpha值
    /// </summary>
    /// <param name="alpha"></param>
    void SetAlpha(float alpha)
    {
        if (renders == null) return;

        currentAlpha = Mathf.Clamp01(alpha);


        foreach (SpriteRenderer sp in renders)
        {
            if (sp != null)
            {
                Color color = sp.color;
                color.a = currentAlpha;
                sp.color = color;
            }
        }

        // if (currentAlpha > 0.9)
        // {
        //     currentAlpha = 1;
        //     currentArea = areaName;
        //     return;
        // }
        // else if (currentAlpha <= 0.1)
        // {
        //     currentAlpha = 0;
        //     currentArea = AreaName.None;
        //     return;
        // }

        if(playerInside == false && currentAlpha >0.5 )
        {
            currentAlpha = 1;
            currentArea = areaName;
            return;
        }
        else if(playerInside == false && currentAlpha <0.5)
        {
            currentAlpha = 0;
            currentArea = AreaName.None;
            return;
        }

    }
}
