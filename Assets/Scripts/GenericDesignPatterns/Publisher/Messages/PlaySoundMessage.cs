using UnityEngine;

public class PlaySoundMessage : IPublisherMessage
{
    public PlaySoundMessage(AudioClip audioClip)
    {
        AudioClip = audioClip;
    }

    public AudioClip AudioClip { get; }
}