using System.Collections;
using UnityEngine;

public class CharingFX_ColorCtrl : MonoBehaviour
{
    [Tooltip("引力触发、锁定颜色的延迟时间（需与 ForceFieldControl 保持一致）")]
    public float lockDelay = 2f;

    private ParticleSystem ps;
    private ParticleSystem.MinMaxGradient originalGradient;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        if (ps != null)
        {
            // 缓存 Inspector 中最初配置好的 Color by Speed 渐变色
            originalGradient = ps.colorBySpeed.color;
        }
    }

    private void OnEnable()
    {
        if (ps == null) return;

        // 对象池每次取出复用时，恢复初始渐变色并启动协程
        var colorBySpeed = ps.colorBySpeed;
        colorBySpeed.color = originalGradient;

        StartCoroutine(CtrlColor());
    }

    private void OnDisable()
    {
        StopAllCoroutines();

        if (ps != null)
        {
            var colorBySpeed = ps.colorBySpeed;
            colorBySpeed.color = originalGradient;
        }
    }

    private IEnumerator CtrlColor()
    {
        yield return new WaitForSeconds(lockDelay);

        var colorBySpeed = ps.colorBySpeed;
        // 取出渐变条最右端（1f 处，即速度降为 0 时）的黄色，将整个 Color by Speed 锁定为该纯色
        Color lockedColor = originalGradient.Evaluate(1f);
        colorBySpeed.color = new ParticleSystem.MinMaxGradient(lockedColor);
    }
}
