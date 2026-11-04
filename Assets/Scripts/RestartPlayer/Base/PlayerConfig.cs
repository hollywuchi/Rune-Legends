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

    [Header("流光闪击参数")]
    public float lightCutFocusCost = 20f;           // 专注值消耗

    [Header("攻击力Buff参数")]
    public float LightCrownFocusCost = 30f;         // 专注值消耗
    public int maxAttackBuffStack = 3;              // 最大叠加层数
    public float attackBuffMultiplier = 2.0f;       // 每层伤害倍率

    [Header("受伤参数")]
    public float hurtFreezeDuration = 0.3f;         // 受伤冻结时间

    [Header("格挡架势参数")]
    public float parryPostureDamageToEnemy = 30f;   // 完美弹反对敌人架势伤害
    public float parryPostureDamageToSelf = 10f;    // 完美弹反对自己架势伤害
    public float blockPostureDamage = 15f;          // 被格挡时受到的架势伤害
    public float parryPostureRecovery = 20f;        // 弹反恢复自身架势值
    public float postureBrokenDuration = 1.5f;      // 破防硬直持续时间
    public float hitStopDuration = 0.05f;           // 弹反Hit Stop时长

    [Header("预输入缓冲时间")]
    public float jumpBufferTime = 0.15f;
    public float attackBufferTime = 0.2f;
    public float sprintBufferTime = 0.15f;
    public float blockBufferTime = 0.15f;

}
