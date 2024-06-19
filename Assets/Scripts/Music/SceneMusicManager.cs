using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMusicManager : MonoBehaviour
{
    public List<AudioClip> musicTracks;
    private AudioSource audioSource;
    private List<AudioClip> remainingTracks;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        remainingTracks = new List<AudioClip>(musicTracks);

        VolumeManager.Instance?.SetCurrentAudioSource(audioSource);

        if (remainingTracks.Count > 0)
        {
            PlayRandomTrack();
        }
    }

    private void PlayRandomTrack()
    {
        if (remainingTracks.Count == 0)
        {
            remainingTracks = new List<AudioClip>(musicTracks);
        }

        int randomIndex = Random.Range(0, remainingTracks.Count);
        audioSource.clip = remainingTracks[randomIndex];
        audioSource.Play();

        remainingTracks.RemoveAt(randomIndex);

        Invoke("PlayRandomTrack", audioSource.clip.length);
    }
}