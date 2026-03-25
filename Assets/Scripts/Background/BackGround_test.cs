using UnityEngine;

// 无限背景
public class BackGround_test : MonoBehaviour
{
    private Camera cam;
    private float bgwidth;

    void Start()
    {
        cam = Camera.main;
        getWidth();
    }

    // void Update()
    // {
    //     bgMove();
    // }

    void LateUpdate()
    {
        bgMove();
    }

    void getWidth()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        bgwidth = spriteRenderer.bounds.size.x;

        // print("图片的宽度：" + bgwidth);
    }

    void bgMove()
    {
        float distence = cam.transform.position.x - transform.position.x;

        if (Mathf.Abs(distence) > bgwidth)
        {
            transform.position += Vector3.right * 2 * bgwidth * Mathf.Sign(distence);
        }
    }
}
