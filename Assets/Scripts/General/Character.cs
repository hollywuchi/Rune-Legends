using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
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
}
