using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Player/PlayerConfig", order = 1)]
public class PlayerConfig : ScriptableObject
{
    [Header("移动参数")]
    public float speed;
    public float sprintSpeed;
    public float jumpForce;
    public float sprintJumpForce;
    // public int maxHealth;
}