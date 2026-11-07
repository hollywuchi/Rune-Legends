using System;
using UnityEngine;

public class Attack : MonoBehaviour
{
   private FocusSystem focusSystem;
   
   [Header("攻击参数")]
   public int Damage = 1;
   public int postureDamage;

   [Header("击退参数")]
   public float knockbackForce = 5f;

   [Header("震屏参数")] 
   public bool shakeOnHit;
   public float shakeForce;
   private bool hasShakenThisSwing;
   [Header("专注获取")] 
   public float focusGain;         // 获得专注值

   private void Awake()
   {
      focusSystem = GetComponentInParent<FocusSystem>();
   }

   private void OnEnable()
   {
      hasShakenThisSwing = false;
   }

   private void OnTriggerEnter2D(Collider2D other)
   {
      var character = other.GetComponent<Character>();
      if(character == null) return;

      int finalDamage = (focusSystem != null && focusSystem.attackBuffStack > 0)
         ? focusSystem.attackBuffStack * focusSystem.attackBuffMultiplier * Damage
         : Damage;
      
      Vector2 knockBackDir = (other.transform.position - transform.position).normalized;

      DamageData data = new DamageData()
      {
         damage = finalDamage, 
         postureDamage = postureDamage,
         knockBackDir = knockBackDir,
         knockBackForce = knockbackForce,
         hitPoint = other.ClosestPoint(other.transform.position),
         attacker = transform.root.gameObject,
         isHeavyAttack = shakeOnHit,
         shakeForce = shakeForce
      };
      
      character.TakeDamage(data);
      // 同步获取的专注值
      if(focusGain != 0)
         EventBus<FocusChangeEvent>.Raise(new FocusChangeEvent() {changeFocusCount = focusGain,isGetFous = true});
   }
}
