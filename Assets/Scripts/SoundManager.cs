using UnityEngine;

public class SoundManager : MonoBehaviour, ISubscriber
{
    AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        Publisher.Subscribe(this, typeof(PlaySoundMessage));
    }

    public void OnDisableSubscriber()
    {
        Publisher.Unsubscribe(this, typeof(PlaySoundMessage));
    }

    public void OnPublish(IPublisherMessage message)
    {
        if (message is PlaySoundMessage soundMessage)
        {
            audioSource.PlayOneShot(soundMessage.AudioClip);
        }
    }
    private void OnDestroy()
    {
        OnDisableSubscriber();
    }
}
