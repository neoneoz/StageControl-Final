using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MobileButtonManager : MonoBehaviour {

    private Button m_button;
	// Use this for initialization
	void Start () {
        m_button = GetComponent<Button>();

	}
	
	// Update is called once per frame
	void Update ()
    {
#if UNITY_ANDROID
        if (m_button.gameObject.activeSelf)
            m_button.gameObject.SetActive(false);
#endif
    }
}
