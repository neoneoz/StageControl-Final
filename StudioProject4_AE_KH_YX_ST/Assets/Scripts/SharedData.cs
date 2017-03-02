using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// Members of class can be accessed in Unity editor
[System.Serializable]
public class Card_Link : System.Object
{
    public CARD_TYPE type;
    public GameObject gm;
}

public class SharedData : MonoBehaviour {
    public static SharedData instance = null;

    public List<Card_Link> DatabasePopulater = null; // All the cards known in the "Stage Control Universe" are registered here
    public SortedList<CARD_TYPE, GameObject> CardDatabase = new SortedList<CARD_TYPE, GameObject>();

    // Background music volume
    [HideInInspector]
    public static float m_bgmVolume; // background music volume is shared among scenes even after scene changes
    public static bool m_changeVolume;
    public static bool m_changeSfx;
    public AudioSource m_bgmObject;
    // Sound effect volume
    public static float m_sfxVolume;

	// Use this for initialization
	void Start ()
    {
        m_bgmObject = GameObject.FindGameObjectWithTag("bgm").GetComponent<AudioSource>(); // Find audio source of background music
	    if(instance == null)
        {
#if UNITY_ANDROID
            Application.targetFrameRate = 30;
            //Application.targetFrameRate = -1;
#elif UNITY_STANDALONE_WIN
            Application.targetFrameRate = -1;
#endif

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
        if (m_changeSfx) // If sound effects is changed by settings screen
        {
            PlayAudio.m_volume = m_sfxVolume; // Change volume
            m_changeSfx = false; // run once until next change
        }

        if (m_changeVolume && m_bgmObject != null)
        {
            m_bgmObject.volume = m_bgmVolume;
            m_changeVolume = false;
        }
        if (m_bgmObject != null && m_bgmVolume != m_bgmObject.volume)
            m_bgmObject.volume = m_bgmVolume;
        else if (/*m_changeVolume &&*/ m_bgmObject == null)
            m_bgmObject = GameObject.FindGameObjectWithTag("bgm").GetComponent<AudioSource>(); // If scene changed, find the background source of dat scene
	}
}
