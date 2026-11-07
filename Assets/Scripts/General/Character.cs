using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

// TODO：添加受击屏幕波纹特效
// TODO:准备重构Character
public class Character : MonoBehaviour
{
   #region 参数部分
   [Header("生命值")]
   public float maxHealth;
   public float CurrentHealth;
   
   [Header("无敌时间")]
   public float invincibleTime;
   private float invincibleTimer;
   public bool invincible => invincibleTimer > 0;
   
   // 事件广播
   public event Action<DamageData> OnHurt;
   public event Action OnDeath;
   public event Action<float, float> OnHealthChange;

   public Func<DamageData, bool> OnTryBlockDamage;
   public event Action<bool, Transform> OnAttackBlocked;

   private void Awake()
   {
      CurrentHealth = maxHealth;
   }

   private void Update()
   {
      if (invincibleTimer > 0)
         invincibleTimer -= Time.deltaTime;
   }

   public void TakeDamage(DamageData data)
   {
      if(invincible || CurrentHealth <= 0) return;

      // 先询问是否有防御状态拦截这次攻击
      if (OnTryBlockDamage != null && OnTryBlockDamage.Invoke(data))
      {
         return;
      }
      
      invincibleTimer = invincibleTime;
      CurrentHealth = Mathf.Max(0, CurrentHealth - data.damage);
      
      OnHealthChange?.Invoke(CurrentHealth,maxHealth);

      if(CurrentHealth == 0)
         OnDeath?.Invoke();
      else
         OnHurt?.Invoke(data);
   }

   public void RestartHealth()
   {
      CurrentHealth = maxHealth;
      OnHealthChange?.Invoke(CurrentHealth,maxHealth);
   }
   
   public void HealthChange(float healCount)
   {
      CurrentHealth = Mathf.Min(CurrentHealth + healCount, maxHealth);
      OnHealthChange?.Invoke(CurrentHealth,maxHealth);
   }

   public void NotifyAttackBlocked(bool isParry,Transform defender)
   {
      OnAttackBlocked?.Invoke(isParry, defender);
   }
   
   

   // [Header("当前存档点")]
   // public Vector3 resurrectPoint;  // 当前复活点位置

   // [Header("特效偏移值")]
   // public Vector3 fxOffset;  // 特效偏移位置

   // [Header("事件监听")]
   // private EventBinding<NewGameEvent> newGameEventBinding;

   // [Header("属性模板")]
   // public PlayerConfig config;
   // [Header("专注值系统")]
   // public float currentFocus;
   // [Header("攻击力buff系统")]
   // public int attackBuffStack = 0;           // 当前攻击力Buff层数
   // private int maxAttackBuffStack = 3;        // 最大叠加层数
   // public float attackBuffMultiplier = 2.0f; // 每层伤害倍率

   // [Header("架势系统")]
   // public PostureSystem postureSystem;  // 架势系统组件引用

   // private SpriteRenderer spriteRenderer;

   // 击退相关
   // private Attack currentAttacker;
   // private Rigidbody2D rb;

   // [Header("Unity事件系统")]
   // private EventBinding<HurtEvent> hurtEventBinding;
   #endregion
   // private void OnEnable()
   // {
   //    spriteRenderer = GetComponent<SpriteRenderer>();
   //    // NewGameEvent.OnEventRaised += NewGame;
   //    newGameEventBinding = new EventBinding<NewGameEvent>(NewGame);
   //    hurtEventBinding = new EventBinding<HurtEvent>(_CreateFX);
   //    hurtEventBinding.Add(KnockBack);
   //
   //    EventBus<HurtEvent>.Register(hurtEventBinding);
   //    EventBus<NewGameEvent>.Register(newGameEventBinding);
   //
   //    postureSystem = GetComponent<PostureSystem>();
   //
   //    // NOTICE:存档系统已被关闭
   //    // ISaveable saveable = this;
   //    // saveable.RegisterSaveData();
   // }
   //
   // private void OnDisable()
   // {
   //    EventBus<NewGameEvent>.Deregister(newGameEventBinding);
   //    EventBus<HurtEvent>.Deregister(hurtEventBinding);
   //    // ISaveable saveable = this;
   //    // saveable.UnRegisterSaveData();
   // }
   //
   // private void NewGame()
   // {
   //    CurrentHealth = maxHealth;
   //    currentFocus = 0;
   // }
   // private void Start()
   // {
   //    rb = GetComponent<Rigidbody2D>();
   //    NewGame();
   // }
   // private void Update()
   // {
   //    if (invincible)
   //    {
   //       invincibleTimer -= Time.deltaTime;
   //       if (invincibleTimer <= 0)
   //       {
   //          invincibleTimer = 0;
   //          invincible = false;
   //       }
   //    }
   // }
   // private void OnTriggerStay2D(Collider2D other)
   // {
   //    if (other.CompareTag("Water"))
   //    {
   //       if (CurrentHealth > 0)
   //       {
   //          CurrentHealth = 0;
   //          EventBus<HealthChangeEvent>.Raise(new HealthChangeEvent { character = this });
   //          EventBus<DeathEvent>.Raise(new DeathEvent { character = this });
   //       }
   //    }
   // }
   //
   // public void TakeDamage(Attack attacker)
   // {
   //    if (invincible)
   //       return;
   //    // 保存攻击者引用用于击退
   //    currentAttacker = attacker;
   //    // 判断当前人物还有没有剩余的血量，没有的话也不用进行判断了，省点性能
   //    if (CurrentHealth - attacker.Damage > 0)
   //    {
   //       CurrentHealth -= attacker.Damage;
   //
   //       //先无敌比较好
   //       invincibleTimer();
   //       
   //       postureSystem?.AddPosture(attacker.postureDamage);
   //       EventBus<HurtEvent>.Raise(new HurtEvent { character = this });
   //       postureSystem?.ResetCombatTimer();
   //    }
   //    else
   //    {
   //       // 宣布死亡
   //       CurrentHealth = 0;
   //       // Death?.Invoke();
   //       EventBus<DeathEvent>.Raise(new DeathEvent { character = this });
   //    }
   //    EventBus<HealthChangeEvent>.Raise(new HealthChangeEvent { character = this });
   // }
   //
   // /// <summary>
   // /// 复活！！！
   // /// </summary>
   // public void Resurrect()
   // {
   //    CurrentHealth = maxHealth;
   //    transform.position = resurrectPoint;
   //    EventBus<HealthChangeEvent>.Raise(new HealthChangeEvent { character = this });
   // }
   //
   // /// <summary>
   // /// 获得专注值
   // /// </summary>
   // public void GainFocus(float amount)
   // {
   //    currentFocus = Mathf.Min(currentFocus + amount, config.maxFocus);
   //    // OnFocusChange?.Invoke(this);
   //    EventBus<FocusChangeEvent>.Raise(new FocusChangeEvent { character = this });
   // }
   //
   // private void _CreateFX(HurtEvent e)
   // {
   //    if (e.character != this) return;
   //    if (tag == null) return;
   //
   //    if (tag == "Enemy")
   //    {
   //       EventBus<FxEvent>.Raise(new FxEvent
   //       {
   //          playerTran = transform.position + fxOffset,
   //          dir = transform.localScale.x,
   //          type = ParticalEffectType.Hit
   //       });
   //    }
   //    if (tag == "Player")
   //    {
   //       StartCoroutine(ColorPrompt());
   //    }
   // }
   // private void KnockBack(HurtEvent e)
   // {
   //    if (e.character != this) return;
   //    if (currentAttacker == null || rb == null) return;
   //
   //    // 计算击退方向：从攻击者指向被攻击者
   //    Vector2 knockbackDir = (transform.position - currentAttacker.transform.position).normalized;
   //
   //    // 应用击退力
   //    rb.velocity = Vector2.zero;
   //    rb.AddForce(knockbackDir * currentAttacker.knockbackForce, ForceMode2D.Impulse);
   // }
   // /// <summary>
   // /// 无敌时间判断
   // /// </summary>
   // public void OnInvincible()
   // {
   //    if (!invincible)
   //    {
   //       invincible = true;
   //       invincibleTimer = invincibleTime;
   //    }
   // }
   //
   // private IEnumerator ColorPrompt()
   // {
   //    if (spriteRenderer == null) yield break;
   //
   //    Color color = new Color(1, 0.5f, 0.5f, 0.5f); // 半透明红色
   //    while (invincible)
   //    {
   //       spriteRenderer.color = color; // 受伤时变红
   //       yield return new WaitForSeconds(0.1f);
   //       spriteRenderer.color = Color.white; // 恢复原色
   //       yield return new WaitForSeconds(0.1f);
   //    }
   //    spriteRenderer.color = Color.white; // 确保恢复原色
   //
   // }
   //
   // #region 数据保存部分
   // public DataDefination GetSaveID() //方便调用方法才用的
   // {
   //    return GetComponent<DataDefination>();
   // }
   //
   // public void GetSaveData(Data data)  //获取保存数据（对比原有的数据，没有就加上）
   // {
   //    if (data.characterPosDic.ContainsKey(GetSaveID().ID))
   //    {
   //       data.characterPosDic[GetSaveID().ID] = transform.position;
   //       data.characterHealth[GetSaveID().ID + "health"] = this.CurrentHealth;   //保存当前血量
   //    }
   //    else
   //    {
   //       data.characterPosDic.Add(GetSaveID().ID, transform.position);
   //       data.characterHealth.Add(GetSaveID().ID + "health", this.CurrentHealth);
   //    }
   // }
   //
   // public void loadData(Data data)  //加载数据
   // {
   //    if (data.characterPosDic.ContainsKey(GetSaveID().ID))
   //    {
   //       transform.position = data.characterPosDic[GetSaveID().ID];
   //       this.CurrentHealth = data.characterHealth[GetSaveID().ID + "health"];
   //    }
   //    // OnHealthChange?.Invoke(this);    //通知UI更新
   //    EventBus<HealthChangeEvent>.Raise(new HealthChangeEvent { character = this });
   //
   // }
   // #endregion
}
