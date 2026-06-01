using UnityEngine;

public class PlayerFxSpeaker : MonoBehaviour
{
    [SerializeField] private FxEventSO fxEventSO;

    public void CreateFX(Vector3 pos, float dir, ParticalEffectType type)
    {
        fxEventSO.RaiseFxEvent(pos, dir, type);
    }

    public void ReliseFX()
    {
        fxEventSO.RaiseReliseFxEvent();
    }
}