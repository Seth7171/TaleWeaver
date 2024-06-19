using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeControlUI : MonoBehaviour
{
    public Slider volumeSlider;

    private void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("volume", 1f);
        volumeSlider.onValueChanged.AddListener(delegate { VolumeManager.Instance.UpdateVolume(volumeSlider.value); });
    }
}
