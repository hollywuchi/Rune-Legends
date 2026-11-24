using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 跨场景 Boss 状态条 UI 控制器：
/// 挂载在常驻场景的 BossUI 物体上（保持物体激活以持续监听 EventBus，通过 CanvasGroup 控制显隐与淡入淡出）。
/// </summary>
public class BossStateBar : MonoBehaviour
{
    [Header("血条 UI")]
    public Image healthImage;
    public Image delayHealthImage;
    public float delaySpeed = 0.25f;

    [Header("架势条 UI")]
    public Image postureImage;

    [Header("淡入淡出")]
    public float fadeInDuration = 0.5f;
    public float fadeOutDuration = 0.5f;

    private CanvasGroup canvasGroup;
    private Character currentBoss;

    private EventBinding<BossBattleStartEvent> battleStartBinding;
    private EventBinding<BossBattleEndEvent> battleEndBinding;
    private EventBinding<PoseChangeEvent> poseChangeBinding;
    private EventBinding<SceneUnloadEvent> sceneUnloadBinding;
    private EventBinding<BackToMenuEvent> backToMenuBinding;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        if (healthImage != null) healthImage.fillAmount = 1f;
        if (delayHealthImage != null) delayHealthImage.fillAmount = 1f;
        if (postureImage != null) postureImage.fillAmount = 0f;
    }

    private void OnEnable()
    {
        battleStartBinding = new EventBinding<BossBattleStartEvent>(OnBossBattleStart);
        battleEndBinding = new EventBinding<BossBattleEndEvent>(HideAndUnbind);
        poseChangeBinding = new EventBinding<PoseChangeEvent>(HandlePostureChange);
        sceneUnloadBinding = new EventBinding<SceneUnloadEvent>(HideAndUnbind);
        backToMenuBinding = new EventBinding<BackToMenuEvent>(HideAndUnbind);

        EventBus<BossBattleStartEvent>.Register(battleStartBinding);
        EventBus<BossBattleEndEvent>.Register(battleEndBinding);
        EventBus<PoseChangeEvent>.Register(poseChangeBinding);
        EventBus<SceneUnloadEvent>.Register(sceneUnloadBinding);
        EventBus<BackToMenuEvent>.Register(backToMenuBinding);
    }

    private void OnDisable()
    {
        EventBus<BossBattleStartEvent>.Deregister(battleStartBinding);
        EventBus<BossBattleEndEvent>.Deregister(battleEndBinding);
        EventBus<PoseChangeEvent>.Deregister(poseChangeBinding);
        EventBus<SceneUnloadEvent>.Deregister(sceneUnloadBinding);
        EventBus<BackToMenuEvent>.Deregister(backToMenuBinding);

        UnbindCurrentBoss();
    }

    private void Update()
    {
        if (healthImage != null && delayHealthImage != null && healthImage.fillAmount < delayHealthImage.fillAmount)
        {
            delayHealthImage.fillAmount = Mathf.MoveTowards(
                delayHealthImage.fillAmount,
                healthImage.fillAmount,
                Time.deltaTime * delaySpeed);
        }
    }

    private void OnDestroy()
    {
        UnbindCurrentBoss();
        if (canvasGroup != null) canvasGroup.DOKill();
        if (postureImage != null) postureImage.DOKill();
    }

    private void OnBossBattleStart(BossBattleStartEvent e)
    {
        UnbindCurrentBoss();
        if (e.bossCharacter == null) return;

        currentBoss = e.bossCharacter;
        currentBoss.OnHealthChange += HandleHealthChange;
        currentBoss.OnDeath += HandleBossDeath;

        float healthRatio = currentBoss.maxHealth > 0f
            ? Mathf.Clamp01(currentBoss.currentHealth / currentBoss.maxHealth)
            : 1f;
        if (healthImage != null) healthImage.fillAmount = healthRatio;
        if (delayHealthImage != null) delayHealthImage.fillAmount = healthRatio;

        float postureRatio = e.bossPosture != null && e.bossPosture.maxPosture > 0f
            ? Mathf.Clamp01(e.bossPosture.currentPosture / e.bossPosture.maxPosture)
            : 0f;
        if (postureImage != null)
        {
            postureImage.DOKill();
            postureImage.fillAmount = postureRatio;
        }

        if (canvasGroup != null)
        {
            canvasGroup.DOKill();
            canvasGroup.DOFade(1f, fadeInDuration);
        }
    }

    private void HandleHealthChange(float current, float max)
    {
        if (healthImage == null || max <= 0f) return;

        healthImage.fillAmount = Mathf.Clamp01(current / max);
        if (delayHealthImage != null && delayHealthImage.fillAmount < healthImage.fillAmount)
        {
            delayHealthImage.fillAmount = healthImage.fillAmount;
        }
    }

    private void HandlePostureChange(PoseChangeEvent e)
    {
        if (currentBoss == null || e.character != currentBoss || postureImage == null || e.maxPosture <= 0f)
        {
            return;
        }

        float ratio = Mathf.Clamp01(e.targetPosture / e.maxPosture);
        postureImage.DOKill();
        if (e.duration > 0f)
        {
            postureImage.DOFillAmount(ratio, e.duration);
        }
        else
        {
            postureImage.fillAmount = ratio;
        }
    }

    private void HandleBossDeath() => HideAndUnbind();

    private void HideAndUnbind()
    {
        UnbindCurrentBoss();

        if (postureImage != null) postureImage.DOKill();
        if (canvasGroup != null)
        {
            canvasGroup.DOKill();
            canvasGroup.DOFade(0f, fadeOutDuration);
        }
    }

    private void UnbindCurrentBoss()
    {
        if (currentBoss == null) return;

        currentBoss.OnHealthChange -= HandleHealthChange;
        currentBoss.OnDeath -= HandleBossDeath;
        currentBoss = null;
    }
}

// TODO：今天要做BOSS的TimeLine以及更炫酷的出场动画
// TODO：完善音效音乐系统，
// TODO：修复存档系统