using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class MirrorCamera : MonoBehaviour
{
    public RenderTexture mirrorRenderTexture;
    public RectTransform mirrorSize;
    private Transform MainCameratrans;
    private PositionConstraint positionConstraint;
    private Camera mirrorCamera;

    void Awake()
    {
        mirrorCamera = GetComponent<Camera>();
    }

    void OnEnable()
    {
        if (mirrorCamera != null)
        {
            Debug.Log("尝试调整相机尺寸");
            mirrorCamera.orthographicSize = Camera.main.orthographicSize;
            mirrorCamera.aspect = Camera.main.aspect;

            // 相机在世界坐标下的真实的宽和高
            float worldHeight = Camera.main.orthographicSize * 2f;
            float worldWidth = worldHeight * Camera.main.aspect;

            Vector3 lossyScale = mirrorSize.lossyScale;
            mirrorSize.sizeDelta = new Vector2(worldWidth / lossyScale.x, worldHeight / lossyScale.y);
            mirrorCamera.targetTexture = mirrorRenderTexture;
        }
    }

    void Start()
    {
        MainCameratrans = Camera.main.transform;
        positionConstraint = GetComponent<PositionConstraint>();

        if (positionConstraint != null && MainCameratrans != null)
            positionConstraint.AddSource(new ConstraintSource { sourceTransform = MainCameratrans, weight = 1 });
    }

}
