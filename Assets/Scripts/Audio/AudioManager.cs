using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("组件")]
    public AudioSource BGMsourse;
    public AudioSource FXSourse;
    public AudioMixer audioMixer;
    [Header("事件广播")]
    public FloatEventSO syncEvent;
    [Header("事件监听")]
    public PlayAudioEventSO BGMEvent;
    public PlayAudioEventSO FXEvent;
    public VoidSo after_PlayAudio;//场景加载结束后该做的事
    public FloatEventSO floatEventSO;
    public VoidSo PauseEvent;
    private void OnEnable()
    {
        FXEvent.OnEventRaised += OnFXEvent;
        BGMEvent.OnEventRaised += OnBGMEvent;
        after_PlayAudio.OnEventRaised += OnPlayBGM;
        floatEventSO.OnEventRaised += OnVolumeEvent;
        PauseEvent.OnEventRaised += OnPause;
    }
    private void OnDisable()
    {
        FXEvent.OnEventRaised -= OnFXEvent;
        BGMEvent.OnEventRaised -= OnBGMEvent;
        after_PlayAudio.OnEventRaised -= OnPlayBGM;
        floatEventSO.OnEventRaised -= OnVolumeEvent;
        PauseEvent.OnEventRaised -= OnPause;
    }

    private void OnPause()
    {
        float Count;
        audioMixer.GetFloat("MasterVolume",out Count);
        syncEvent.RaiseEvent(Count);
    }

    private void OnVolumeEvent(float Count)
    {
        audioMixer.SetFloat("MasterVolume", Count);
    }

    /// <summary>
    /// 场景加载结束之后该播放BGM了
    /// 是否可以找到“BGM”找到就播放
    /// </summary>
    private void OnPlayBGM()
    {
        GameObject.Find("BGM")?.GetComponent<AudioDefination>().PlayAudioClip();
    }

    private void OnBGMEvent(AudioClip BgmClip)
    {
        BGMsourse.clip = BgmClip;
        BGMsourse.Play();
    }

    private void OnFXEvent(AudioClip audioClip)
    {
        FXSourse.clip = audioClip;
        FXSourse.Play();
    }
}
