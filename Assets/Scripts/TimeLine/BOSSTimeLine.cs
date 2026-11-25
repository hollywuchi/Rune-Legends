using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class BOSSTimeLine : MonoBehaviour
{
    public PlayableDirector director;

    public Character bossChar;
    public PostureSystem bossPosture;

    public BehaviorTreeRunner beRunner;
    private CinemachineBrain cameraBrain;

    private void Awake()
    {
        if (cameraBrain == null)
            cameraBrain = Camera.main?.GetComponent<CinemachineBrain>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        GetMianCam();
        director?.gameObject.SetActive(true);
    }

    private void GetMianCam()
    {
        if (director == null) return;

        foreach (var output in director.playableAsset.outputs)
        {
            if (output.sourceObject is CinemachineTrack)
            {
                director.SetGenericBinding(output.sourceObject, cameraBrain);
            }
        }
    }


    public void ShackCam()
    {
        EventBus<HeavyAtkCameraShakeEvent>.Raise(new HeavyAtkCameraShakeEvent { Force = 2 });
    }

    public void ShowUI()
    {
        if (bossChar == null) return;

        EventBus<BossBattleStartEvent>.Raise(new BossBattleStartEvent
        {
            bossCharacter = bossChar,
            bossPosture = bossPosture
        });
    }

    public void FinishInto()
    {
        if (beRunner == null) return;
        beRunner.isInto = true;
    }
}