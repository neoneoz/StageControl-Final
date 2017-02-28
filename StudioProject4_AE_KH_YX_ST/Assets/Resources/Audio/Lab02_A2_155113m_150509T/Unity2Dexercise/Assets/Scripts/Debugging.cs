using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Debugging : MonoBehaviour {

    public Text debugtxt;

	// Use this for initialization
	void Start () {
        debugtxt.text = "Started";
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ClickedButtA () {
        debugtxt.text = "ButtA Clicked";
    }

    public void ClickedButtB () {
        debugtxt.text = "ButtB Clicked";
    }

    public void Quit()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                Application.Quit();
            }
        }
    }
}
