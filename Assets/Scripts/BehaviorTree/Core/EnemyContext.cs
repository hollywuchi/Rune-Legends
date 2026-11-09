using UnityEngine;

public class EnemyContext
{
    public GameObject gameObject;
    public Transform transform;
    public Enemy enemy;
    public Rigidbody2D rb;
    public Animator anim;
    public PhysicsCheck physicsCheck;
    public PostureSystem postureSystem;

    public static EnemyContext CreateFromObject(GameObject _go)
    {
        var context = new EnemyContext();
        context.gameObject = _go;
        context.transform = _go.GetComponent<Transform>();
        context.enemy = _go.GetComponent<Enemy>();
        context.rb = _go.GetComponent<Rigidbody2D>();
        context.anim = _go.GetComponent<Animator>();
        context.physicsCheck = _go.GetComponent<PhysicsCheck>();
        context.postureSystem = _go.GetComponent<PostureSystem>();
        return context;
    }
}
        