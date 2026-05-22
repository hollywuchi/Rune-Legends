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

   private void OnTriggerEnter2D(Collider2D other)
   {
      other.GetComponent<Character>()?.TakeDamage(this);
   }
}
