using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour, ISaveable
{
   #region 参数部分
   [Header("事件监听")]
   public VoidSo NewGameEvent;
   [Header("基本属性")]
   //总血量    
   public float maxHealth;
   //当前血量
   public float CurrentHealth;
   // 无敌时间
   public float invincibleTime;
   float invincibleCounter;
   public bool invincible;

   public UnityEvent<Character> OnHealthChange;
   public UnityEvent<Transform> Hurt;
   public UnityEvent Death;
   #endregion
   private void OnEnable()
   {
      NewGameEvent.OnEventRaised += NewGame;
   }


   private void OnDisable()
   {
      NewGameEvent.OnEventRaised -= NewGame;
      ISaveable saveable = this;
      saveable.UnRegisterSaveData();
   }
   private void NewGame()
   {
      CurrentHealth = maxHealth;
   }
   private void Start()
   {
      NewGame();
      ISaveable saveable = this;
      saveable.RegisterSaveData();
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
      // 判断当前人物还有没有剩余的血量，没有的话也不用进行判断了，省点性能
      if (CurrentHealth - attacker.Damage > 0)
      {
         CurrentHealth -= attacker.Damage;
         //先无敌比较好
         invincibleTimer();
         Hurt?.Invoke(attacker.transform);
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
