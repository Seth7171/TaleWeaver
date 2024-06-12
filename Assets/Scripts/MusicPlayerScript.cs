using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    private AudioSource AudioSource;
    private float musicVolume = 1f;

    public Slider volumeSlider;
    public GameObject ObjectMusic;

    void Start()
    {
        ObjectMusic = GameObject.FindWithTag("GameMusic");
        AudioSource = ObjectMusic.GetComponent<AudioSource>();

        musicVolume = PlayerPrefs.GetFloat("volume");
        AudioSource.volume = musicVolume;
        volumeSlider.value = musicVolume;
    }

    // Update is called once per frame
    void Update()
    {
        AudioSource.volume = musicVolume;
        PlayerPrefs.SetFloat("volume", musicVolume);
    }


    public void updateVolume(float volume)
    {
        musicVolume = volume;
    }


}
