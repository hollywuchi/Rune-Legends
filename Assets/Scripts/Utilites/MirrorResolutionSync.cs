using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class MirrorResolutionSync : MonoBehaviour
{
    [Header("相机引用")]
    public Camera mainCamera;       // 玩家主相机
    public Camera reflectionCamera; // 专门用于渲染镜像的相机

    [Header("性能设置")]
    [Range(0.1f, 1f)]
    [Tooltip("降低镜像分辨率以提升性能，0.5代表屏幕分辨率的一半")]
    public float resolutionScale = 1f; 

    private RawImage rawImage;
    private RectTransform rectTransform;
    private RenderTexture currentRT;

    private int lastScreenWidth = 0;
    private int lastScreenHeight = 0;

    void Awake()
    {
        mainCamera = Camera.main.GetComponent<Camera>();
        rawImage = GetComponent<RawImage>();
        rectTransform = GetComponent<RectTransform>();
        
        if (mainCamera == null) mainCamera = Camera.main;
    }

    void Update()
    {
        // 实时检测屏幕分辨率/窗口大小是否发生变化
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            UpdateMirrorSetup();
        }
    }

    void UpdateMirrorSetup()
    {
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        // 计算目标 RT 的分辨率
        int targetWidth = Mathf.RoundToInt(Screen.width * resolutionScale);
        int targetHeight = Mathf.RoundToInt(Screen.height * resolutionScale);

        // 1. 处理 RenderTexture 重建，防止画面拉伸
        if (currentRT != null)
        {
            reflectionCamera.targetTexture = null;
            rawImage.texture = null;
            currentRT.Release();
            Destroy(currentRT);
        }

        // 创建新的 RT (ARGB32 保证透明度通道正常)
        currentRT = new RenderTexture(targetWidth, targetHeight, 16, RenderTextureFormat.ARGB32);
        currentRT.filterMode = FilterMode.Bilinear;
        
        reflectionCamera.targetTexture = currentRT;
        rawImage.texture = currentRT;

        // 2. 处理 UI 世界坐标尺寸适配，防止黑边或溢出
        if (mainCamera.orthographic)
        {
            float orthoSize = mainCamera.orthographicSize;
            float worldHeight = orthoSize * 2f;
            float worldWidth = worldHeight * mainCamera.aspect;

            // 截图中你的 Canvas 带有 0.01111111 的 Scale
            // 这里我们需要除以 lossyScale，确保 sizeDelta 乘上 Scale 后刚好等于世界尺寸
            Vector3 scale = rectTransform.lossyScale;
            
            rectTransform.sizeDelta = new Vector2(
                worldWidth / scale.x, 
                worldHeight / scale.y
            );
        }
    }

    void OnDestroy()
    {
        if (currentRT != null)
        {
            currentRT.Release();
        }
    }
}