using UnityEngine;

public class PlayerFxSpeaker : MonoBehaviour
{
    [SerializeField] private FxEventSO fxEventSO;

    public void CreateFX(Transform trans, float dir, ParticalEffectType type)
    {
        fxEventSO.RaiseFxEvent(trans, dir, type);
    }
}