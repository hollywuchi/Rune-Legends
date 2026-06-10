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
         Character playerCharacter = GetComponentInParent<Character>();
         if (playerCharacter != null && CompareTag("Attack"))
         {
            Damage = 1;
            playerCharacter.GainFocus(focusGain);
            Damage += (int)(playerCharacter.attackBuffStack * playerCharacter.attackBuffMultiplier);
         }

         // 检测目标是否在格挡状态
         PlayerBlockState blockState = GetBlockState(other);
         if (blockState != null)
         {
            // 目标正在格挡
            HandleBlockInteraction(character, blockState, playerCharacter);
         }
         else
         {
            // 普通伤害
            character.TakeDamage(this);
         }
      }
   }

   /// <summary>
   /// 获取目标的格挡状态（如果正在格挡）
   /// </summary>
   private PlayerBlockState GetBlockState(Collider2D other)
   {
      // 检查是否是玩家
      Player player = other.GetComponent<Player>();
      if (player != null)
      {
         return player.stateMachine.currentState as PlayerBlockState;
      }
      return null;
   }

   // REVIEW：格挡系统 - 处理格挡交互：判断弹反或普通格挡，并通知敌人
   private void HandleBlockInteraction(Character targetCharacter, PlayerBlockState blockState, Character attackerCharacter)
   {
      // 获取敌人组件（Attack collider 是 Enemy 的子物体）
      Enemy enemy = GetComponentInParent<Enemy>();

      if (blockState.IsInParryWindow())
      {
         // 弹反成功！
         // 对攻击者造成架势伤害
         if (attackerCharacter != null && attackerCharacter.postureSystem != null)
         {
            attackerCharacter.postureSystem.AddPosture(targetCharacter.config.parryPostureDamageToEnemy);
         }

         // 减少目标自身架势值
         if (targetCharacter.postureSystem != null)
         {
            targetCharacter.postureSystem.ReducePosture(targetCharacter.config.parryPostureRecovery);
         }

         // 通知格挡状态弹反成功
         blockState.OnBlocked(true);

         // 通知敌人被弹反
         enemy?.OnBlockedByPlayer(true, targetCharacter.transform);
      }
      else
      {
         // 普通格挡，不对目标造成伤害，但是对目标造成架势伤害
         if (targetCharacter.postureSystem != null)
         {
            targetCharacter.postureSystem.AddPosture(targetCharacter.config.blockPostureDamage);
         }

         // 同时也会对自己造成架势伤害
         if (attackerCharacter != null && attackerCharacter.postureSystem != null)
         {
            attackerCharacter.postureSystem.AddPosture(targetCharacter.config.parryPostureDamageToSelf);
         }

         // 通知格挡状态普通格挡
         blockState.OnBlocked(false);

         // 通知敌人被格挡
         enemy?.OnBlockedByPlayer(false, targetCharacter.transform);
      }
   }
}
