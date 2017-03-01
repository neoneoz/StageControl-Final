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

    // Combat and explosion sound effects   
    public AudioClip m_ballista;
    public AudioClip m_buster;
    public AudioClip m_buildingExplode;
    public AudioClip m_clockworkKnight;
    public AudioClip m_ironGolem;
    public AudioClip m_railgun;
    public AudioClip m_railgunExplode;
    public AudioClip m_spidertank;
    public AudioClip m_drawCard;
    public AudioClip m_hoverEnter;
    public AudioClip m_hoverExit;
    public AudioClip m_newDeck;
    public AudioSource m_source;
    public GameObject m_soundOwner; // Used to find out which entity is playing the sound
    // Total volume here, uses slider to adjust all in game volume
    public static float m_volume = 1;
	// Use this for initialization
	void Start () {
        m_source = GetComponent<AudioSource>();
        m_ballista = Resources.Load("Audio/BallistaBoltFire") as AudioClip;
        m_buster = Resources.Load("Audio/Bbuster") as AudioClip;
        m_buildingExplode = Resources.Load("Audio/building explode") as AudioClip;
        m_clockworkKnight = Resources.Load("Audio/CwKnight") as AudioClip;
        m_ironGolem = Resources.Load("Audio/irongolem") as AudioClip;
        m_railgun = Resources.Load("Audio/railgun") as AudioClip;
        m_railgunExplode = Resources.Load("Audio/railgunexplode") as AudioClip;
        m_spidertank = Resources.Load("Audio/spidertank") as AudioClip;
        m_drawCard = Resources.Load("Audio/cardFlip") as AudioClip;
        m_hoverEnter = Resources.Load("Audio/HoverEnter") as AudioClip;
        m_hoverExit = Resources.Load("Audio/HoverExit") as AudioClip;
        m_newDeck = Resources.Load("Audio/NewDeckSound") as AudioClip;
        DontDestroyOnLoad(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
        if (m_volume <= 0)
        {
            m_source.volume = 0;
        }
        else if (m_volume <= m_source.volume)
        {
            m_volume = m_source.volume + 0.1f;
        }
        else if (m_volume > 0 && m_volume <= 1)
        {
            float originalValue = m_source.volume;
            m_source.volume *= m_volume; // if sound clip volume for railgun is set to 0.3, when it is multiplied by 0.4, it is 0.12, effectively lowering the volume
            if (m_source.volume != originalValue)
                m_source.volume += originalValue;
            if (m_source.volume > 1)
                m_source.volume = 1;
            else if (m_source.volume < 0)
                m_source.volume = 0;
        }
    }

    public void PlayOnce()
    {
        m_source.PlayOneShot(m_source.clip);
    }
}
