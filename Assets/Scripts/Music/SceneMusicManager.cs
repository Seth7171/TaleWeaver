using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMusicManager : MonoBehaviour
{
    public List<AudioClip> musicTracks;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (musicTracks.Count > 0)
        {
            PlayRandomTrack();
        }
    }

    private void PlayRandomTrack()
    {
        if (musicTracks.Count == 0)
            return;

        int randomIndex = Random.Range(0, musicTracks.Count);
        audioSource.clip = musicTracks[randomIndex];
        audioSource.Play();

        musicTracks.RemoveAt(randomIndex); // Remove the played track from the list

        // Play the next random track when the current one finishes
        Invoke("PlayRandomTrack", audioSource.clip.length);
    }
}
