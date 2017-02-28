using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class JoyCTRL : MonoBehaviour {

    public Text printOut;
    private Image ImgFG;
    private Image ImgBG;

	// Use this for initialization
	void Start () {
        ImgBG = GetComponent<Image>();
        ImgFG = transform.GetChild(0).GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Dragging()
    {
#if UNITY_ANDROID
        Touch myTouch = Input.GetTouch(0);
         Vector3 newPos = new Vector3(myTouch.position.x, myTouch.position.y, 1);
         ImgFG.rectTransform.position = newPos;
#else
        Vector3 newPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
        ImgFG.rectTransform.position = newPos;
        printOut.text = "x: " + Input.mousePosition.x + "y: " + Input.mousePosition.y;
#endif
    }

    public void ReturnOrigin()
    {
        ImgFG.rectTransform.anchoredPosition = new Vector3(0, 0, 1);
    }
}
