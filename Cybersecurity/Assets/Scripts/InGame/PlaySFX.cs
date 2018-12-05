using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySFX : MonoBehaviour
{
    public void PlayOneShot(AudioClip audioClip)
    {
        if (audioClip == null)
            return;

        if (AudioPlayer.Instance == null)
            return;

        AudioPlayer.Instance.PlaySFXOneShot(audioClip);
    }

    public void PlaySFXFromAnimation(Object audioClipObject)
    {
        if (audioClipObject != null)
            PlayOneShot((AudioClip)audioClipObject);
    }
}
