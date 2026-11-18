using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
   [Header("生命值")]
   public float maxHealth;
   public float currentHealth;
   
   [Header("无敌时间")]
   public float invincibleTime;
   private float invincibleTimer;
   private bool Invincible => invincibleTimer > 0;
   
   // 事件广播
   public event Action<DamageData> OnHurt;
   public event Action OnDeath;
   public event Action<float, float> OnHealthChange;

   public Func<DamageData, bool> OnTryBlockDamage;
   public event Action<bool, Transform> OnAttackBlocked;

   [HideInInspector] public bool isHurt;

   private void Awake()
   {
      currentHealth = maxHealth;
   }

   private void Update()
   {
      if (invincibleTimer > 0)
         invincibleTimer -= Time.deltaTime;
   }

   public void TakeDamage(DamageData data)
   {
      if(Invincible || currentHealth <= 0) return;

      // 先询问是否有防御状态拦截这次攻击
      if (OnTryBlockDamage != null && OnTryBlockDamage.Invoke(data))
      {
         return;
      }
      
      invincibleTimer = invincibleTime;
      
      currentHealth = Mathf.Max(0, currentHealth - data.damage);
      
      OnHealthChange?.Invoke(currentHealth,maxHealth);

      if(currentHealth == 0)
         OnDeath?.Invoke();
      else
         OnHurt?.Invoke(data);
   }

   public void RestartHealth()
   {
      currentHealth = maxHealth;
      OnHealthChange?.Invoke(currentHealth,maxHealth);
   }
   
   public void HealthChange(float healCount)
   {
      currentHealth = Mathf.Min(currentHealth + healCount, maxHealth);
      OnHealthChange?.Invoke(currentHealth,maxHealth);
   }

   public void NotifyAttackBlocked(bool isParry,Transform defender)
   {
      OnAttackBlocked?.Invoke(isParry, defender);
   }
   
   /// <summary>
   /// 在这期间无敌
   /// </summary>
   /// <param name="duration"></param>
   public void SetInvincibleTime(float duration)
   {
      invincibleTimer = Mathf.Max(invincibleTime, duration);
   }
}
