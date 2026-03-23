using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioDefination : MonoBehaviour
{
    public PlayAudioEventSO playAudioEvent;
    public AudioClip audioClip;
    public bool PlayOnEnable;
    private void OnEnable() 
    {
        if(PlayOnEnable)
            PlayAudioClip();
    }
    public void PlayAudioClip()
    {
        playAudioEvent.RaisedEvent(audioClip);
    }
}
