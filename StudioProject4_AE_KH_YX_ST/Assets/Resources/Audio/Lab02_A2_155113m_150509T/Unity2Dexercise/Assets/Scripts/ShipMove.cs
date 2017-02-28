using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShipMove : MonoBehaviour {

    private Image ship;

	// Use this for initialization
	void Start () {
	    ship = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
        Touch tch;
        Vector3 wrldpoint;
        bool hit = false;
        if (Input.touchCount == 1)
        {
            tch = Input.GetTouch(0);
            wrldpoint = Camera.main.ScreenToWorldPoint(tch.position);
            wrldpoint.z = -1f;
            if (tch.phase == TouchPhase.Began)
            {
                if (Physics.Raycast(wrldpoint, Vector3.forward))
                {
                    hit = true;
                }
            }
            else if (hit && (tch.phase == TouchPhase.Moved))
            {
                wrldpoint.z = 0f;
                ship.transform.position = wrldpoint;
            }
            else if (tch.phase == TouchPhase.Ended)
            {
                hit = false;
            }
        }
	}
}
