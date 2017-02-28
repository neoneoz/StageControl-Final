using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Card_Link : System.Object
{
    public CARD_TYPE type;
    public GameObject gm;
}

public class SharedData : MonoBehaviour {
    public static SharedData instance = null;

    public List<Card_Link> DatabasePopulater = null;
    public SortedList<CARD_TYPE, GameObject> CardDatabase = new SortedList<CARD_TYPE, GameObject>();

    // Background music volume
    [HideInInspector]
    public static float m_bgmVolume;
    public static bool m_changeVolume;
    public static bool m_changeSfx;
    public AudioSource m_bgmObject;
    // Sound effect volume
    public static float m_sfxVolume;

	// Use this for initialization
	void Start ()
    {
        m_bgmObject = GameObject.FindGameObjectWithTag("bgm").GetComponent<AudioSource>();
	    if(instance == null)
        {
            if (DatabasePopulater == null)
            {
                Debug.Log("Database populater is null");
            }

            foreach(Card_Link link in DatabasePopulater)
            {
                CardDatabase.Add(link.type, link.gm);
                //Debug.Log(link.type.ToString() + "  " + CardDatabase.Count);
            }
            instance = this;

            if (DatabasePopulater != null)
                DatabasePopulater.Clear();

            m_bgmVolume = 0.5f;
            m_sfxVolume = 1f;
            DontDestroyOnLoad(this.gameObject);

        }else
        {
            Destroy(gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (m_changeSfx)
        {
            PlayAudio.m_volume = m_sfxVolume;
            m_changeSfx = false;
        }

        if (m_changeVolume && m_bgmObject != null)
        {
            m_bgmObject.volume = m_bgmVolume;
            m_changeVolume = false;
        }
        if (m_bgmObject != null && m_bgmVolume != m_bgmObject.volume)
            m_bgmObject.volume = m_bgmVolume;
        else if (/*m_changeVolume &&*/ m_bgmObject == null)
            m_bgmObject = GameObject.FindGameObjectWithTag("bgm").GetComponent<AudioSource>();
	}
}
