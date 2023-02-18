using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class SFXPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioSource audioSource2;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (!audioSource)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource2 = gameObject.AddComponent<AudioSource>();
    }

    public void PlayAudioLoop(AudioClip clip)
    {
        if (audioSource.clip == clip && audioSource.isPlaying) return;

        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void PlayAudio(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.loop = false;
        audioSource.Play();
    }

    public void PlayAudioSecond(AudioClip clip)
    {
        audioSource2.clip = clip;
        audioSource2.loop = false;
        audioSource2.Play();
    }

    public void PlayRandomAudio(AudioClip[] clips) =>
        PlayAudio(clips[Random.Range(0, clips.Length)]);
}
