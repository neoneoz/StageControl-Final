using UnityEngine;
using System.Collections;

public class PlayAudio : MonoBehaviour {
    private static PlayAudio m_instance;
    public static PlayAudio instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject go = new GameObject("PlayAudioController");
                m_instance = go.AddComponent<PlayAudio>();
                m_instance.m_source = go.AddComponent<AudioSource>();
            }
            return m_instance;
        }
    }
    public AudioClip m_ballista;
    public AudioSource m_source;
    public GameObject m_soundOwner; // Used to find out which entity is playing the sound
    // Put volume here, then use slider to adjust all in game volume
	// Use this for initialization
	void Start () {
        m_source = GetComponent<AudioSource>();
        m_ballista = Resources.Load("Audio/BallistaBoltFire") as AudioClip;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void PlayOnce()
    {
        m_source.PlayOneShot(m_source.clip);
    }
}
