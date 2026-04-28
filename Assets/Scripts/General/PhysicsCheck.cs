using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    // 地面检测部分
    [Header("物理材质")]
    public PhysicsMaterial2D Wall;
    public PhysicsMaterial2D Walk;
    [Header("检测参数")]
    public bool Auto;
    public float PhyRadius;
    public LayerMask CheckLayer;
    public Vector2 GroundOffset;
    public Vector2 LeftOffset;
    public Vector2 rightOffset;
    public Vector2 leftTopOffset;
    public Vector2 rightTopOffset;
    [Header("状态")]
    public bool IsGround;
    public bool touchLeftWall;
    public bool touchRightWall;
    public bool touchLeftTopWall;
    public bool touchRightTopWall;

    BoxCollider2D coll;
    private void Awake()
    {
        coll = GetComponent<BoxCollider2D>();
        if (Auto)
        {
            AutoChangeOffset();
        }
    }
    void Update()
    {
        CheckGround();
        if (IsGround)
        {
            coll.sharedMaterial = Walk;
        }
        else
        {
            coll.sharedMaterial = Wall;
        }
    }
    /// <summary>
    /// 地面检测方法，使用的是，physice2D碰撞检测中的球形碰撞检测
    /// 官方的解释是：检测一个碰撞器是否落在一个圆形的区域（里面）
    /// 其中使用的参数是：这个园的圆心，半径，要检测的部分
    /// 这里放置一个新的参数offset是为了更好的调整检测的位置
    /// </summary>
    void CheckGround()
    {
        IsGround = Physics2D.OverlapCircle((Vector2)transform.position + GroundOffset, PhyRadius, CheckLayer);
        // 左右墙面的检测
        touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + LeftOffset, PhyRadius, CheckLayer);
        touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, PhyRadius, CheckLayer);
        touchRightTopWall = Physics2D.OverlapCircle((Vector2)transform.position + rightTopOffset, PhyRadius, CheckLayer);
        touchLeftTopWall = Physics2D.OverlapCircle((Vector2)transform.position + leftTopOffset, PhyRadius, CheckLayer);
    }
    // 现在绘制以上的碰撞区域
    // 使用的就是一个辅助的方法，好在Unity帮我们弄好了这些
    // 这两个方法都可以绘制，上面是选择才会绘制，下面是永久绘制（会让界面显得很乱）
    private void OnDrawGizmosSelected()
    {
        // 甚至还可以更改颜色
        Gizmos.color = Color.yellow;
        // 画一个圆形（因为上面的碰撞是圆形碰撞）(参数同样是圆心与范围)
        Gizmos.DrawWireSphere((Vector2)transform.position + GroundOffset, PhyRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + LeftOffset, PhyRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, PhyRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftTopOffset, PhyRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightTopOffset, PhyRadius);

    }
    // 这里就是无论什么时候都可以绘制（永久出现）
    // private void OnDrawGizmos() 
    // {

    // }
    public void AutoChangeOffset()
    {
        rightOffset = new Vector2(coll.bounds.size.x / 2 + coll.offset.x, coll.size.y / 2);
        LeftOffset = new Vector2(-coll.bounds.size.x / 2 + coll.offset.x, coll.size.y / 2);
    }

}
