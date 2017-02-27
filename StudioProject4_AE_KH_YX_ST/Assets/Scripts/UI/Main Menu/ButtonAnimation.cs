using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonAnimation : MonoBehaviour {
    Button m_button;
	// Use this for initialization
	void Start () {
        m_button = GetComponent<Button>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void Hover()
    {
        ///m_button.image.rectTransform.sizeDelta = new Vector2(m_button.image.rectTransform.sizeDelta.x * 2, m_button.image.rectTransform.sizeDelta.y * 2);
    }
}
