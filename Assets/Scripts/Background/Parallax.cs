using UnityEngine;

public class Parallax : MonoBehaviour
{
    private Transform camTrans;
    private Vector3 lastPos;

    [SerializeField] private float parallaxSpeed = 1f;

    void Start()
    {
        camTrans = Camera.main.transform;
        lastPos = camTrans.position;
    }

    void LateUpdate()
    {
        ParallaxMove();
    }

    private void ParallaxMove()
    {
        float deltaX = camTrans.position.x - lastPos.x;
        transform.position += new Vector3(deltaX * parallaxSpeed, 0, 0);
        lastPos = camTrans.position;
    }


}