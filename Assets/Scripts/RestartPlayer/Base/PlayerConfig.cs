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

}
