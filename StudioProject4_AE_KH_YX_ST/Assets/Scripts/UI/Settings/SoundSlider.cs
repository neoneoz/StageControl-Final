using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SoundSlider : MonoBehaviour {
    private Slider m_slider; // The slider this script is attached to, to get slider properties like min and max values
	// Use this for initialization
	void Start () {
        m_slider = GetComponent<Slider>();
        if (gameObject.tag == "bgm") // If this is the bgm slider, make sure game will be able to know later also
        {
            SharedData.m_bgmVolume = m_slider.value;
            SharedData.m_changeVolume = true;
        }
        else if (gameObject.tag == "sfx") // If this is the bgm slider, make sure game will be able to know later also
        {
            SharedData.m_sfxVolume = m_slider.value;
            SharedData.m_changeSfx = true;
        }
        m_slider.onValueChanged.AddListener(delegate { ValueChangeCheck(); }); // When value changes on slider, call the function delegated
	}
	
	// Update is called once per frame
	void Update () {

	}

    void ValueChangeCheck()
    {
        if (gameObject.tag == "bgm")
        {
            SharedData.m_bgmVolume = m_slider.value;
            SharedData.m_changeVolume = true;
        }
        else if (gameObject.tag == "sfx")
        {
            SharedData.m_sfxVolume = m_slider.value;
            SharedData.m_changeSfx = true;
        }
    }
}
