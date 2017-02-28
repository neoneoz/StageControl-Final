using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Options : MonoBehaviour {

    public Slider music;
    public Slider SFX;
    public AudioSource soundBG;
    public AudioSource soundSFX;

    public float musicVolume = 100;
    public float SFXVolume = 100;
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void changeMusicVolume()
    {
        musicVolume = music.value;
        musicVolume = musicVolume / 100;
        soundBG.volume = musicVolume;
    }

    public void changeSFXVolume()
    {
        SFXVolume = SFX.value;
        SFXVolume = SFXVolume / 100;
        soundSFX.volume = SFXVolume;
    }
}
