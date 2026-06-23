using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class MirrorCamera : MonoBehaviour
{
    public RenderTexture mirrorRenderTexture;
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
            mirrorCamera.orthographicSize = Camera.main.orthographicSize;
            if(mirrorRenderTexture != null && (mirrorRenderTexture.width != Screen.width || mirrorRenderTexture.height != Screen.height))
            {
                mirrorRenderTexture.width = Screen.width;
                mirrorRenderTexture.height = Screen.height;
            }

            mirrorCamera.targetTexture = mirrorRenderTexture;
        }
    }

    void Start()
    {
        // MainCameratrans = GameObject.FindGameObjectWithTag("MainCamera").transform;
        MainCameratrans = Camera.main.transform;
        positionConstraint = GetComponent<PositionConstraint>();

        if(positionConstraint != null && MainCameratrans != null)
            positionConstraint.AddSource(new ConstraintSource{sourceTransform = MainCameratrans, weight = 1});
    }

}
