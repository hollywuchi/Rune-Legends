using UnityEngine;

/// <summary>
/// 所有组件的上下文，只有叶子节点会读取这个上下文并获取组件
/// </summary>
public class EnemyContext
{
    public GameObject GameObject;
    public Transform Transform;
    public EnemyMotor Motor;
    public EnemyConfig Config;
    public Animator Anim;
    public PhysicsCheck PhysicsCheck;
    public PostureSystem PostureSystem;
    public Character Character;

    public static EnemyContext CreateFromObject(GameObject _go)
    {
        var context = new EnemyContext
        {
            GameObject = _go,
            Transform = _go.GetComponent<Transform>(),
            Motor = _go.GetComponent<EnemyMotor>(),
            Config = _go.GetComponent<EnemyConfig>(),
            Anim = _go.GetComponent<Animator>(),
            PhysicsCheck = _go.GetComponent<PhysicsCheck>(),
            PostureSystem = _go.GetComponent<PostureSystem>(),
            Character = _go.GetComponent<Character>()
        };
        return context;
    }
}
        