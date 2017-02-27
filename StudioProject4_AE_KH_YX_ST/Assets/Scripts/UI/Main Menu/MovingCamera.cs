using UnityEngine;
using System.Collections;

public class MovingCamera : MonoBehaviour {
    public GameObject m_lookAt; // GameObject with a position which camera will be at 
    public float m_moveSpeed = 0.1f; // Speed at which camera will move
    public GameObject m_mainCanvas; // The main canvas of the scene, also the canvas that has Screen Space Overlay
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = Vector3.Lerp(transform.position, m_lookAt.transform.position, m_moveSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, m_lookAt.transform.rotation, m_moveSpeed);
	}

    public void ChangeMount(GameObject otherLookat)
    {
        m_lookAt = otherLookat;
        if (!otherLookat.name.Equals("LookAtMain"))
            m_mainCanvas.SetActive(false);
        else
            m_mainCanvas.SetActive(true);
    }
}
