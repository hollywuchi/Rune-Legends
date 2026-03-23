using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPoint : MonoBehaviour,IInteractable
{
    public SceneLoadEventSO loadEventSO;
    public GameSceneSO SceneToGo;
    public Vector3 PositionToGo;
    public void TriggerAction()
    {
        loadEventSO.RaiseLoadRequestEvent(SceneToGo,PositionToGo,true);
    }
}
