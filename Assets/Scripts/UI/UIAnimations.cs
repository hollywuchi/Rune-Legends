using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UIAnimations : MonoBehaviour
{
    [Header("UI 核心对象")]
    [Tooltip("中心大石头。大石头上的文字和滑块请作为此对象的子节点。")]
    public RectTransform mainIsland;
    public RectTransform[] satelliteIslands; // 卫星小石头数组
    public TextMeshProUGUI[] satelliteTexts;            // 卫星小石头上的文字数组
    [Tooltip("挂载在父节点上，用于统一控制所有UI元素的淡入淡出")]
    public CanvasGroup uiCanvasGroup;

    [Header("待机与交互设置")]
    [SerializeField] private float idleFloatAmplitude = 5f; // 垂直浮动幅度（像素）
    [SerializeField] private float idleFloatDuration = 3f;  // 浮动周期
    [SerializeField] private float hoverMoveDistance = 10f; // 悬停时的排斥距离
    [SerializeField] private float hoverScaleFactor = 1.1f; // 悬停时的放大因子
    [SerializeField] private Color hoverColor = new Color(0.0f, 1.0f, 1.0f, 1.0f); // 亮青色
    [SerializeField] private float selectMoveDistance = -5f; // 点击时的确认收缩距离

    [Header("进出场设置")]
    [SerializeField] private float enterDropOffset = 600f; // 大石头进场前的垂直隐藏偏移量

    private Vector2 mainIslandOriginPos;
    private Vector2[] satelliteOriginPositions;
    private Color[] satelliteOriginColors;
    private float[] satelliteYOffsets = { 0.2f, 0.5f, 0.8f, 1.1f }; // 卫星岛浮动相位差（制造错落感）

    private Sequence currentSequence;
    private bool isInteractable = false; // 交互锁：防止在进出场动画期间触发交互


    private void Awake()
    {
        // 初始化组件和界面
        InitializeOriginPoints();

    }

    void OnEnable()
    {
        // 每次启用时都播放进场动画，确保界面状态正确
        PlayEnterAnimation();
    }
    

    private void InitializeOriginPoints()
    {
        // 记录大石头的初始位置
        mainIslandOriginPos = mainIsland.anchoredPosition;

        satelliteOriginPositions = new Vector2[satelliteIslands.Length];
        satelliteOriginColors = new Color[satelliteTexts.Length];

        // 记录周围小石头的位置和文字初始颜色
        for (int i = 0; i < satelliteIslands.Length; i++)
        {
            satelliteOriginPositions[i] = satelliteIslands[i].anchoredPosition;
            satelliteOriginColors[i] = satelliteTexts[i].color;
        }
    }

    // --- 1. 进出场动画 ---
    public void PlayEnterAnimation()
    {
        if (currentSequence != null && currentSequence.IsActive()) currentSequence.Kill();
        isInteractable = false; // 进场期间禁用交互

        // 状态重置
        uiCanvasGroup.alpha = 0f;
        mainIsland.anchoredPosition = new Vector2(mainIslandOriginPos.x, mainIslandOriginPos.y - enterDropOffset);

        for (int i = 0; i < satelliteIslands.Length; i++)
        {
            satelliteIslands[i].anchoredPosition = mainIslandOriginPos; // 隐藏在大石头中心
            satelliteIslands[i].localScale = Vector3.zero;
        }

        currentSequence = DOTween.Sequence();

        currentSequence.SetUpdate(true); // 确保动画在 Time.timeScale = 0 时仍然播放（如暂停状态）

        // 阶段 1: 大石头升起
        currentSequence.Append(mainIsland.DOAnchorPos(mainIslandOriginPos, 0.5f).SetEase(Ease.OutBack, 1.2f));

        // 阶段 2: 卫星小石头射出 (在大石头快到位时触发)
        for (int i = 0; i < satelliteIslands.Length; i++)
        {
            currentSequence.Insert(0.3f, satelliteIslands[i].DOAnchorPos(satelliteOriginPositions[i], 0.5f).SetEase(Ease.OutCubic));
            currentSequence.Insert(0.3f, satelliteIslands[i].DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack));
        }

        // 阶段 3: 整体透明度淡入
        currentSequence.Insert(0.6f, uiCanvasGroup.DOFade(1f, 0.4f));

        // 动画结束回调
        currentSequence.OnComplete(() =>
        {
            isInteractable = true; // 允许交互
            StartIdleAnimations(); // 开启待机漂浮
        });
    }

    public void PlayExitAnimation(System.Action onCompleteCallback = null)
    {
        if (currentSequence != null && currentSequence.IsActive()) currentSequence.Kill();
        isInteractable = false; // 离场期间禁用交互

        // 停止所有待机和交互残留的动画
        mainIsland.DOKill();
        foreach (var island in satelliteIslands) island.DOKill();
        foreach (var text in satelliteTexts) text.DOKill();

        currentSequence = DOTween.Sequence();

        currentSequence.SetUpdate(true); // 确保动画在 Time.timeScale = 0 时仍然播放（如暂停状态）

        // 阶段 1: 整体透明度淡出
        currentSequence.Append(uiCanvasGroup.DOFade(0f, 0.2f));

        // 阶段 2: 卫星小石头被吸回大石头中心
        for (int i = 0; i < satelliteIslands.Length; i++)
        {
            currentSequence.Insert(0.1f, satelliteIslands[i].DOAnchorPos(mainIslandOriginPos, 0.3f).SetEase(Ease.InBack));
            currentSequence.Insert(0.1f, satelliteIslands[i].DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack));
        }

        // 阶段 3: 大石头掉落
        Vector2 dropTargetPos = new Vector2(mainIslandOriginPos.x, mainIslandOriginPos.y - enterDropOffset);
        currentSequence.Insert(0.3f, mainIsland.DOAnchorPos(dropTargetPos, 0.4f).SetEase(Ease.InSine));

        // 动画结束回调
        currentSequence.OnComplete(() =>
        {
            onCompleteCallback?.Invoke();
        });
    }

    // --- 2. 待机动画 ---

    private void StartIdleAnimations()
    {
        // 大石头缓慢浮动
        mainIsland.DOAnchorPosY(mainIslandOriginPos.y + idleFloatAmplitude, idleFloatDuration)
                  .SetLoops(-1, LoopType.Yoyo)
                  .SetEase(Ease.InOutSine)
                  .SetUpdate(true);

        // 小石头错位缓慢浮动
        for (int i = 0; i < satelliteIslands.Length; i++)
        {
            float adjustedAmplitude = idleFloatAmplitude * 0.7f;
            satelliteIslands[i].DOAnchorPosY(satelliteOriginPositions[i].y + adjustedAmplitude, idleFloatDuration)
                              .SetLoops(-1, LoopType.Yoyo)
                              .SetEase(Ease.InOutSine)
                              .SetDelay(satelliteYOffsets[i]) // 利用延迟制造不同步感
                              .SetUpdate(true);
        }
    }

    // // --- 3. 鼠标交互事件 ---

    // public void OnHoverEnterSatellite(int index)
    // {
    //     if (!isInteractable || index < 0 || index >= satelliteIslands.Length) return;

    //     satelliteIslands[index].DOKill();
    //     satelliteTexts[index].DOKill();

    //     Vector2 direction = (satelliteOriginPositions[index] - mainIslandOriginPos).normalized;
    //     Vector2 targetPos = satelliteOriginPositions[index] + direction * hoverMoveDistance;

    //     Sequence hoverSeq = DOTween.Sequence();
    //     hoverSeq.SetUpdate(true);
    //     hoverSeq.Append(satelliteIslands[index].DOAnchorPos(targetPos, 0.2f).SetEase(Ease.OutBack));
    //     hoverSeq.Join(satelliteIslands[index].DOScale(hoverScaleFactor, 0.2f).SetEase(Ease.OutBack));
    //     hoverSeq.Join(satelliteTexts[index].DOColor(hoverColor, 0.2f));
    // }

    // public void OnHoverExitSatellite(int index)
    // {
    //     if (!isInteractable || index < 0 || index >= satelliteIslands.Length) return;

    //     satelliteIslands[index].DOKill();
    //     satelliteTexts[index].DOKill();

    //     Sequence exitSeq = DOTween.Sequence();
    //     exitSeq.SetUpdate(true);
    //     exitSeq.Append(satelliteIslands[index].DOAnchorPos(satelliteOriginPositions[index], 0.2f).SetEase(Ease.InSine));
    //     exitSeq.Join(satelliteIslands[index].DOScale(1.0f, 0.2f).SetEase(Ease.InSine));
    //     exitSeq.Join(satelliteTexts[index].DOColor(satelliteOriginColors[index], 0.2f));

    //     // 恢复原位后，重新加入待机浮动循环
    //     exitSeq.OnComplete(() => {
    //         satelliteIslands[index].DOAnchorPosY(satelliteOriginPositions[index].y + idleFloatAmplitude * 0.7f, idleFloatDuration)
    //                           .SetLoops(-1, LoopType.Yoyo)
    //                           .SetEase(Ease.InOutSine)
    //                           .SetDelay(satelliteYOffsets[index])
    //                           .SetUpdate(true);
    //     });
    // }

    // public void OnSelectSatellite(int index)
    // {
    //     if (!isInteractable || index < 0 || index >= satelliteIslands.Length) return;

    //     satelliteIslands[index].DOKill();
    //     satelliteTexts[index].DOKill();

    //     Sequence selectSeq = DOTween.Sequence();
    //     selectSeq.SetUpdate(true);
    //     Vector2 shrinkPos = satelliteOriginPositions[index] + (satelliteIslands[index].anchoredPosition - mainIslandOriginPos).normalized * selectMoveDistance;

    //     selectSeq.Append(satelliteIslands[index].DOAnchorPos(shrinkPos, 0.1f).SetEase(Ease.InSine));
    //     selectSeq.Append(satelliteIslands[index].DOAnchorPos(satelliteOriginPositions[index], 0.1f).SetEase(Ease.OutBack));
    //     selectSeq.Join(satelliteTexts[index].DOColor(satelliteOriginColors[index], 0.1f));

    //     selectSeq.OnComplete(() => {
    //         Debug.Log($"确认点击：小石头选项 {index}");
    //         // 在此处调用 GameManager 或实际的 UI 切换逻辑
    //     });
    // }
}