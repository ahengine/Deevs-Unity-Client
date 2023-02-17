using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] musics;
    [SerializeField] private float voulmeFadeDuration = .2f;
    private int musicIndex;
    private AudioSource audioSource;
    private float volume;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        volume = audioSource.volume;
        Play();
    }

    public void Play()
    {
        audioSource.clip = musics[musicIndex];
        audioSource.Play();
    }

    public void PlayNextMusic()
    {
        musicIndex = musicIndex < musics.Length -1 ? musicIndex + 1 : 0;
        Play();
        if(audioSource.volume == 0)
            ChangeVolume(volume);
    }

    public void Stop() => ChangeVolume(0,audioSource.Stop);

    private void ChangeVolume(float volume,Action OnComplete = null)
    {
        if (ChangeVolumeCoroutine != null) StopCoroutine(ChangeVolumeCoroutine);
        ChangeVolumeCoroutine = StartCoroutine(ChangeVolumeFade(volume, OnComplete));
    }

    private Coroutine ChangeVolumeCoroutine;
    private IEnumerator ChangeVolumeFade(float volume, Action OnComplete)
    {
        float timer = 0;
        float start = audioSource.volume;
        while(timer < voulmeFadeDuration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, volume, timer / voulmeFadeDuration);
            yield return new WaitForEndOfFrame();
        }

        audioSource.volume = volume;
        OnComplete?.Invoke();
    }
}
