using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Enemy", order = 0)]
public class EnemyConfig : ScriptableObject
{
    public float maxHealth;
    public float maxPosture;

    public float normalSpeed;
    public float chaseSpeed;

    public float loseTargetTime;
}