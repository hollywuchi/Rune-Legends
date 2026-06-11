using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour, ISaveable
{
   #region 参数部分
   [Header("生命值与无敌时间")]
   public float maxHealth;
   //当前血量
   public float CurrentHealth;
   public float invincibleTime;
   float invincibleCounter;
   public bool invincible;
   [Header("当前存档点")]
   public Vector3 resurrectPoint;  // 当前复活点位置

   [Header("特效偏移值")]
   public Vector3 fxOffset;  // 特效偏移位置

   [Header("事件监听")]
   public VoidSo NewGameEvent;
   [Header("事件广播")]
   public FxEventSO fxEventSO;
   [Header("属性模板")]
   public PlayerConfig config;
   [Header("专注值系统")]
   public float currentFocus;
   [Header("攻击力buff系统")]
   public int attackBuffStack = 0;           // 当前攻击力Buff层数
   private int maxAttackBuffStack = 3;        // 最大叠加层数
   public float attackBuffMultiplier = 2.0f; // 每层伤害倍率

   [Header("架势系统")]
   public PostureSystem postureSystem;  // 架势系统组件引用

   private SpriteRenderer spriteRenderer;

   // 击退相关
   private Attack currentAttacker;
   private Rigidbody2D rb;

   public UnityEvent<Character> OnHealthChange;
   public UnityEvent<Character> OnFocusChange;
   public UnityEvent Hurt;
   public UnityEvent Death;
   #endregion
   private void OnEnable()
   {
      spriteRenderer = GetComponent<SpriteRenderer>();
      NewGameEvent.OnEventRaised += NewGame;
      Hurt.AddListener(_CreateFX);
      Hurt.AddListener(KnockBack);
      postureSystem = GetComponent<PostureSystem>();
      // ISaveable saveable = this;
      // saveable.RegisterSaveData();
   }

   private void OnDisable()
   {
      NewGameEvent.OnEventRaised -= NewGame;
      Hurt.RemoveAllListeners();
      // ISaveable saveable = this;
      // saveable.UnRegisterSaveData();
   }

   private void NewGame()
   {
      CurrentHealth = maxHealth;
      currentFocus = 0;
   }
   private void Start()
   {
      rb = GetComponent<Rigidbody2D>();
      NewGame();
   }
   private void Update()
   {
      if (invincible)
      {
         invincibleCounter -= Time.deltaTime;
         if (invincibleCounter <= 0)
         {
            invincibleCounter = 0;
            invincible = false;
         }
      }
   }
   private void OnTriggerStay2D(Collider2D other)
   {
      if (other.CompareTag("Water"))
      {
         if (CurrentHealth > 0)
         {
            CurrentHealth = 0;
            OnHealthChange?.Invoke(this);
            Death?.Invoke();
         }
      }
   }

   public void TakeDamage(Attack attacker)
   {
      if (invincible)
         return;
      // 保存攻击者引用用于击退
      currentAttacker = attacker;
      // 判断当前人物还有没有剩余的血量，没有的话也不用进行判断了，省点性能
      if (CurrentHealth - attacker.Damage > 0)
      {
         CurrentHealth -= attacker.Damage;

         //先无敌比较好
         invincibleTimer();
         Hurt?.Invoke();
      }
      else
      {
         // 宣布死亡
         CurrentHealth = 0;
         Death?.Invoke();
      }
      OnHealthChange?.Invoke(this);
   }

   /// <summary>
   // REVIEW：格挡系统 - 被格挡时的伤害处理（减伤 + 架势伤害）
   /// 被格挡时的伤害处理（减伤 + 架势伤害）
   /// </summary>
   // public void TakeBlockedDamage(Attack attacker, float damageReduction)
   // {
   //    if (invincible) return;

   //    currentAttacker = attacker;

   //    // 计算减伤后的伤害
   //    int reducedDamage = Mathf.Max(1, Mathf.RoundToInt(attacker.Damage * (1f - damageReduction)));

   //    if (CurrentHealth - reducedDamage > 0)
   //    {
   //       CurrentHealth -= reducedDamage;
   //       invincibleTimer();
   //    }
   //    else
   //    {
   //       CurrentHealth = 0;
   //       Death?.Invoke();
   //    }
   //    OnHealthChange?.Invoke(this);
   // }

   /// <summary>
   /// 复活！！！
   /// </summary>
   public void Resurrect()
   {
      CurrentHealth = maxHealth;
      transform.position = resurrectPoint;
      OnHealthChange?.Invoke(this);
   }

   /// <summary>
   /// 获得专注值
   /// </summary>
   public void GainFocus(float amount)
   {
      currentFocus = Mathf.Min(currentFocus + amount, config.maxFocus);
      OnFocusChange?.Invoke(this);
   }

   private void _CreateFX()
   {
      if (tag == null) return;

      if (tag == "Enemy" && fxEventSO != null)
      {
         fxEventSO.RaiseFxEvent(transform.position + fxOffset, transform.localScale.x, ParticalEffectType.Hit);
      }
      if (tag == "Player")
      {
         StartCoroutine(ColorPrompt());
      }
      // fxSpeaker.CreateFX(transform, transform.position.x, ParticalEffectType.Hit);
   }
   private void KnockBack()
   {
      if (currentAttacker == null || rb == null) return;

      // 计算击退方向：从攻击者指向被攻击者
      Vector2 knockbackDir = (transform.position - currentAttacker.transform.position).normalized;

      // 应用击退力
      rb.velocity = Vector2.zero;
      rb.AddForce(knockbackDir * currentAttacker.knockbackForce, ForceMode2D.Impulse);
   }
   /// <summary>
   /// 无敌时间判断
   /// </summary>
   public void invincibleTimer()
   {
      if (!invincible)
      {
         invincible = true;
         invincibleCounter = invincibleTime;
      }
   }

   private IEnumerator ColorPrompt()
   {
      if (spriteRenderer == null) yield break;

      Color color = new Color(1, 0.5f, 0.5f, 0.5f); // 半透明红色
      while (invincible)
      {
         spriteRenderer.color = color; // 受伤时变红
         yield return new WaitForSeconds(0.1f);
         spriteRenderer.color = Color.white; // 恢复原色
         yield return new WaitForSeconds(0.1f);
      }
      spriteRenderer.color = Color.white; // 确保恢复原色

   }

   #region 数据保存部分
   public DataDefination GetSaveID() //方便调用方法才用的
   {
      return GetComponent<DataDefination>();
   }

   public void GetSaveData(Data data)  //获取保存数据（对比原有的数据，没有就加上）
   {
      if (data.characterPosDic.ContainsKey(GetSaveID().ID))
      {
         data.characterPosDic[GetSaveID().ID] = transform.position;
         data.characterHealth[GetSaveID().ID + "health"] = this.CurrentHealth;   //保存当前血量
      }
      else
      {
         data.characterPosDic.Add(GetSaveID().ID, transform.position);
         data.characterHealth.Add(GetSaveID().ID + "health", this.CurrentHealth);
      }
   }

   public void loadData(Data data)  //加载数据
   {
      if (data.characterPosDic.ContainsKey(GetSaveID().ID))
      {
         transform.position = data.characterPosDic[GetSaveID().ID];
         this.CurrentHealth = data.characterHealth[GetSaveID().ID + "health"];
      }
      OnHealthChange?.Invoke(this);    //通知UI更新
   }
   #endregion
}
