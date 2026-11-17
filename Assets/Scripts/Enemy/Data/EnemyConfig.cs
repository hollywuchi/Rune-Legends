using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Enemy", order = 0)]
public class EnemyConfig : ScriptableObject
{
    public float maxHealth;
    public float maxPosture;

    public float normalSpeed;
    public float chaseSpeed;
    
    [Header("脱战时间")]
    public float loseTargetTime;

    [Header("巡逻等待时间")] 
    public float waitingTime;   // 撞墙等待时间
    public float patrolTime;    // 最长巡逻时间
}