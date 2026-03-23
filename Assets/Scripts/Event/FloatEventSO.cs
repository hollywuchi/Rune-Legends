using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/FloatEventSO")]
public class FloatEventSO : ScriptableObject
{
    public UnityAction<float> OnEventRaised;
    public void RaiseEvent(float Count)
    {
        OnEventRaised?.Invoke(Count);
    }
}
