using UnityEngine;

/// <summary>
/// 在游戏中产生的数据都会被写在黑板上，所有的敌人都会知道黑板上的内容并做出行动
/// </summary>
public class Blackboard
{
    public Transform TargetPlayer;
    public Vector2 TargetPosition;

    public float LoseTargetTimer;
    public bool IsVision;

    public bool HasPlayedIntro;
}