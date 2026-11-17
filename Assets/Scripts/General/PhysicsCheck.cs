using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    // 地面检测部分
    [Header("物理材质")] public PhysicsMaterial2D Wall;
    public PhysicsMaterial2D Walk;
    [Header("检测参数")] public float phyRadius;
    public LayerMask checkLayer;
    public Vector2 leftGroundOffset;
    public Vector2 rightGroundOffset;
    public Vector2 leftOffset;
    public Vector2 rightOffset;
    public Vector2 leftTopOffset;
    public Vector2 rightTopOffset;
    [Header("状态")] public bool isGround;
    public bool isLeftGround;
    public bool isRightGround;
    public bool touchLeftWall;
    public bool touchRightWall;
    public bool touchLeftTopWall;
    public bool touchRightTopWall;

    BoxCollider2D coll;

    private void Awake()
    {
        coll = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        CheckGround();
        coll.sharedMaterial = isGround ? Walk : Wall;
    }

    /// <summary>
    /// 地面检测方法，使用的是，physice2D碰撞检测中的球形碰撞检测
    /// 官方的解释是：检测一个碰撞器是否落在一个圆形的区域（里面）
    /// 其中使用的参数是：这个园的圆心，半径，要检测的部分
    /// 这里放置一个新的参数offset是为了更好的调整检测的位置
    /// </summary>
    private void CheckGround()
    {
        isGround = isLeftGround && isRightGround;

        isLeftGround = Physics2D.OverlapCircle((Vector2)transform.position + leftGroundOffset, phyRadius, checkLayer);
        isRightGround = Physics2D.OverlapCircle((Vector2)transform.position + rightGroundOffset, phyRadius, checkLayer);
        // 左右墙面的检测
        touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, phyRadius, checkLayer);
        touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, phyRadius, checkLayer);
        touchRightTopWall = Physics2D.OverlapCircle((Vector2)transform.position + rightTopOffset, phyRadius, checkLayer);
        touchLeftTopWall = Physics2D.OverlapCircle((Vector2)transform.position + leftTopOffset, phyRadius, checkLayer);
    }

    // 现在绘制以上的碰撞区域
    // 使用的就是一个辅助的方法，好在Unity帮我们弄好了这些
    // 这两个方法都可以绘制，上面是选择才会绘制，下面是永久绘制（会让界面显得很乱）
    private void OnDrawGizmosSelected()
    {
        // 甚至还可以更改颜色
        Gizmos.color = Color.yellow;
        // 画一个圆形（因为上面的碰撞是圆形碰撞）(参数同样是圆心与范围)
        Gizmos.DrawWireSphere((Vector2)transform.position + leftGroundOffset, phyRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightGroundOffset, phyRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, phyRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, phyRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftTopOffset, phyRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightTopOffset, phyRadius);
    }

    private bool TouchWall_R()
    {
        return transform.localScale.x < 0 && touchLeftWall || transform.localScale.x > 0 && touchRightWall;
    }

    private bool TouchWall_L()
    {
        return transform.localScale.x > 0 && touchLeftWall || transform.localScale.x < 0 && touchRightWall;
    }

    private bool IsGround_R()
    {
        return transform.localScale.x < 0 && !isLeftGround || transform.localScale.x > 0 && !isRightGround;
    }

    private bool IsGround_L()
    {
        return transform.localScale.x > 0 && !isLeftGround || transform.localScale.x < 0 && !isRightGround;
    }

    public bool IsFrontGround(bool facingRight)
    {
        return facingRight ? IsGround_R() : IsGround_L();
    }

    public bool TouchFrontWall(bool facingRight)
    {
        return facingRight ? TouchWall_R() : TouchWall_L();
    }
}