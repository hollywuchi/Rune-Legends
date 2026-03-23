using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//攻击的脚本
public class Attack : MonoBehaviour
{
   // 攻击的数值
   public int Damage;
   // 攻击的范围
   public float attackRange;
   // 攻击的频率
   public float attackRate;


   //不仅仅是怪物，玩家同样也可以应用这个脚本，
   //因此这个脚本要挂载到所有可以攻击的物体上，当有角色触碰到这个脚本的时候，
   //攻击的数值和频率会进行运算然后传递给角色，角色再进行自身生命值的更改
    
    /// <summary>
    /// Sent each frame where another object is within a trigger collider
    /// attached to this object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    private void OnTriggerStay2D(Collider2D other)
    {
      // 使用三目运算符保证安全运行
      other.GetComponent<Character>()?.TakeDamage(this);
    }
}
