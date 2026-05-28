using UnityEngine;

public class Attack : MonoBehaviour
{
   [Header("攻击参数")]
   public int Damage = 1;
   public float attackRange;
   public float attackRate;

   [Header("击退参数")]
   public float knockbackForce = 5f;
   public float knockbackDuration = 0.2f;

   [Header("专注获取")]
   public bool canGainFocus = true;  // 是否可以获得专注值
   public float focusGain = 7.5f;         // 获得专注值

   private void OnTriggerEnter2D(Collider2D other)
   {
      Character character = other.GetComponent<Character>();
      if (character != null)
      {
         character.TakeDamage(this);

         Character playerCharacter = GetComponentInParent<Character>();
         if (playerCharacter != null) playerCharacter.GainFocus(focusGain);
      }
   }
}
