using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 架势系统组件，可挂载到玩家和敌人上
/// 管理架势值的增减、恢复、破防判定
/// </summary>
public class PostureSystem : MonoBehaviour
{
    [Header("架势参数")]
    public float maxPosture;

    [Header("脱战衰减机制")]
    public float outOfCombatDelay;        // 脱战时间
    public float postureDecayRate;        // 衰减速率

    [Header("破防状态")]
    // TODO:继续做破防衰减状态
    public float postureRecoveryRate;     // 每秒减去的架势值
    public float postureRecoveryDelay;   // 破防后持续时间，期间无法增加架势值

    public float currentPosture;  // 当前的架势值
    [HideInInspector] public bool isBroken;         // 是否处于破防状态
    [HideInInspector] public float recoveryTimer;   // 恢复计时器

    public UnityEvent OnPostureBroken;
    public UnityEvent OnPostureRecovered;

    private Character character;

    void Awake()
    {
        character = GetComponent<Character>();
    }

    private void OnEnable()
    {
        currentPosture = 0f;
        isBroken = false;
        recoveryTimer = 0f;
    }

    private void Update()
    {
        if (!isBroken) return;

        if (currentPosture > 0f)
        {
            recoveryTimer -= Time.deltaTime;
            if (recoveryTimer <= 0f)
            {
                currentPosture -= postureRecoveryRate * Time.deltaTime;
                if (currentPosture < 0f)
                {
                    currentPosture = 0f;
                    ResetPosture();
                }
            }
        }
    }

    /// <summary>
    /// 增加架势值
    /// </summary>
    public void AddPosture(float amount)
    {
        if (isBroken) return;

        currentPosture = Mathf.Min(currentPosture + amount, maxPosture);
        recoveryTimer = postureRecoveryDelay;

        if (currentPosture >= maxPosture)
        {
            BreakPosture();
        }

        if (character == null) return;
        EventBus<PoseChangeEvent>.Raise(new PoseChangeEvent
        {
            character = character,
            targetPosture = currentPosture,
            maxPosture = maxPosture,
            duration = 0.5f
        });
    }

    /// <summary>
    /// 减少架势值（弹反成功时调用）
    /// </summary>
    public void ReducePosture(float amount)
    {
        currentPosture = Mathf.Max(currentPosture - amount, 0f);

        if (character == null) return;
        EventBus<PoseChangeEvent>.Raise(new PoseChangeEvent
        {
            character = character,
            targetPosture = currentPosture,
            maxPosture = maxPosture,
            duration = 0.5f
        });
    }

    /// <summary>
    /// 破防处理
    /// </summary>
    private void BreakPosture()
    {
        isBroken = true;
        currentPosture = maxPosture;
        OnPostureBroken?.Invoke();
    }

    /// <summary>
    /// 破防结束，重置架势
    /// </summary>
    public void ResetPosture()
    {
        isBroken = false;
        currentPosture = 0f;
        recoveryTimer = 0f;
        OnPostureRecovered?.Invoke();
        EventBus<PoseChangeEvent>.Raise(new PoseChangeEvent
        {
            character = character,
            targetPosture = currentPosture,
            maxPosture = maxPosture,
            duration = 0.5f
        });
    }

}
