using System;
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
    public float brokenDuration;            // 破防之后的大硬直时间

    public float currentPosture;  // 当前的架势值
    [HideInInspector] public bool isBroken;         // 是否处于破防状态
    private float combatTimer;      // 脱战倒计时
    private float brokenTimer;   // 破防恢复计时器

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
        brokenTimer = 0f;
        combatTimer = 0f;

        if (character != null)
            character.OnHurt += PoseHurt;
    }

    private void OnDisable()
    {
        if (character != null)
            character.OnHurt -= PoseHurt;
    }


    private void Update()
    {
        if (isBroken)
        {
            UpdateBroken();
            return;
        }

        UpdateDecay();
    }

    /// <summary>
    /// 增加架势值
    /// </summary>
    public void AddPosture(float amount)
    {
        if (isBroken) return;

        currentPosture = Mathf.Min(currentPosture + amount, maxPosture);

        ResetCombatTimer();

        if (currentPosture >= maxPosture)
        {
            BreakPosture();
            return;
        }

        PostureChange(0.2f);
    }
    
    private void PoseHurt(DamageData data)
    {
        AddPosture(data.postureDamage);
    }

    /// <summary>
    /// 破防实现架势重置
    /// </summary>
    private void UpdateBroken()
    {
        brokenTimer -= Time.deltaTime;
        if (brokenTimer <= 0f)
        {
            ResetPosture();
        }
    }

    /// <summary>
    /// 没破防，正常脱战
    /// </summary>
    private void UpdateDecay()
    {
        if (currentPosture <= 0) return;

        if (combatTimer > 0)
        {
            combatTimer -= Time.deltaTime;
            return;
        }

        float prePoseture = currentPosture;
        currentPosture = Mathf.Max(0, currentPosture - postureDecayRate * Time.deltaTime);
        
        if(Mathf.Abs(prePoseture-currentPosture) > 0.001f)
        {
            PostureChange(0f);
        }
    }

    /// <summary>
    /// 减少架势值（弹反成功时调用）
    /// </summary>
    public void ReducePosture(float amount)
    {
        if(isBroken) return;
        currentPosture = Mathf.Max(currentPosture - amount, 0f);

        PostureChange(0.2f);
    }

    private void ResetCombatTimer()
    {
        combatTimer = outOfCombatDelay;
    }

    /// <summary>
    /// 破防处理
    /// </summary>
    private void BreakPosture()
    {
        isBroken = true;
        currentPosture = maxPosture;
        brokenTimer = brokenDuration;

        OnPostureBroken?.Invoke();
        PostureChange(0.1f);
    }

    /// <summary>
    /// 破防结束，重置架势
    /// </summary>
    private void ResetPosture()
    {
        isBroken = false;
        currentPosture = 0f;
        brokenTimer = 0f;
        OnPostureRecovered?.Invoke();
        PostureChange(0.2f);
    }

    private void PostureChange(float duration)
    {
        if(character == null) return;

        EventBus<PoseChangeEvent>.Raise(new PoseChangeEvent{
            character = character,
            targetPosture = currentPosture,
            maxPosture = maxPosture,
            duration = duration
        });
    }
}
