using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicLogic : MonoBehaviour
{
    private AudioSource _music; 
    private AudioSource _sfx;
    public Slider MusicSlider;
    public Slider SfxSlider;

    private void Awake()
    {
        _music = GetComponent<AudioSource>();
        _music.volume = 1.0f;
        _sfx = GetComponent<AudioSource>();
        _sfx.volume = 1.0f;
    }


    public void MusicVol()
    {
        _music.volume = MusicSlider.value;
    }

    public void SfxVol()
    {
        _sfx.volume = SfxSlider.value;
    }
}
