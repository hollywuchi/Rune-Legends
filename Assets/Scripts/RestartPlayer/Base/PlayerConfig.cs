using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Player/PlayerConfig", order = 1)]
public class PlayerConfig : ScriptableObject
{
    [Header("基本属性")]
    public float maxHealth;
    public float maxFocus;
    public float healAmount;

    [Header("移动参数")]
    public float speed;
    public float sprintSpeed;
    public float jumpForce;
    public float wallJumpForce;
    public float wallSlideSpeed;
    public float wallHorizontalBoost;

    [Header("攻击参数")]
    public float attackComboWindow = 0.6f;      // 连招窗口时间（秒）
    public float attackMoveDistance = 0.5f;      // 攻击位移距离
    public float attackCooldown = 0.1f;          // 攻击冷却时间
    public int attackDamage = 1;                 // 基础攻击伤害
    public float attackKnockbackForce = 5f;      // 击退力
    
    [Header("连招伤害倍率")]
    public float combo1DamageMultiplier = 1.0f;  // 第一段伤害倍率
    public float combo2DamageMultiplier = 1.2f;  // 第二段伤害倍率
    public float combo3DamageMultiplier = 1.5f;  // 第三段伤害倍率

    [Header("霹雳一闪参数")]
    public float lightCutFocusCost = 20f;           // 专注值消耗
    public float lightCutDashDistance = 5.0f;       // 冲刺距离
    public float lightCutDashSpeed = 20.0f;         // 冲刺速度

    [Header("攻击力Buff参数")]
    public float LightCrownFocusCost = 30f;         // 专注值消耗
    public int maxAttackBuffStack = 3;              // 最大叠加层数
    public float attackBuffMultiplier = 2.0f;       // 每层伤害倍率

    [Header("受伤参数")]
    public float hurtFreezeDuration = 0.3f;         // 受伤冻结时间

    // REVIEW：格挡系统 - 格挡架势参数
    [Header("格挡架势参数")]
    public float maxPosture = 100f;                 // 最大架势值
    public float postureRecoveryRate = 15f;         // 架势恢复速率（每秒）
    public float postureRecoveryDelay = 1.5f;       // 受击后恢复延迟
    public float blockDamageReduction = 0.8f;       // 格挡减伤比例（80%减伤）
    public float blockPostureDamage = 15f;          // 被格挡时受到的架势伤害
    public float parryWindowDuration = 0.12f;       // 弹反窗口时长（秒）
    public float parryPostureDamageToEnemy = 30f;   // 弹反对敌人架势伤害
    public float parryPostureRecovery = 20f;        // 弹反恢复自身架势值
    public float postureBrokenDuration = 1.5f;      // 破防硬直持续时间
    public float hitStopDuration = 0.05f;           // 弹反Hit Stop时长
    public float parryScreenShakeIntensity = 0.3f;  // 弹反屏幕震动强度
    public float parryScreenShakeDuration = 0.15f;  // 弹反屏幕震动时长

}
