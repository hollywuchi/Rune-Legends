using System;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour, ISaveable
{
   #region 参数部分
   [Header("事件监听")]
   public VoidSo NewGameEvent;
   [Header("事件广播")]
   public FxEventSO fxEventSO;
   [Header("属性模板")]
   public PlayerConfig config;
   [Header("基本属性")]
   public Vector3 fxOffset;   //特效偏移
   //总血量    
   public float maxHealth;
   //当前血量
   public float CurrentHealth;
   // 无敌时间
   public float invincibleTime;
   float invincibleCounter;
   public bool invincible;

   // 击退相关
   private Attack currentAttacker;
   private Rigidbody2D rb;

   // 专注系统
   public float currentFocus;
   public UnityEvent<Character> OnHealthChange;
   public UnityEvent Hurt;
   public UnityEvent Death;
   #endregion
   private void OnEnable()
   {
      NewGameEvent.OnEventRaised += NewGame;
      Hurt.AddListener(_CreateFX);
      Hurt.AddListener(KnockBack);
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

      // REVIEW:双份的打断治愈状态
      // var player = GetComponent<Player>();
      // if (player != null && player.ctx.IsHealing)
      // {
      //    player.ctx.IsHealing = false;
      // }

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
   /// 获得专注值
   /// </summary>
   public void GainFocus(float amount)
   {
      currentFocus = Mathf.Min(currentFocus + amount, config.maxFocus);
   }

   private void _CreateFX()
   {
      if (tag == null) return;

      if (tag == "Enemy" && fxEventSO != null)
      {
         fxEventSO.RaiseFxEvent(transform.position + fxOffset, transform.localScale.x, ParticalEffectType.Hit);
      }
      // if( tag == "Player")
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
