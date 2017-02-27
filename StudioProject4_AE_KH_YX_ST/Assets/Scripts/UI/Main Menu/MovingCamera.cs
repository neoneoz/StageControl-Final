using UnityEngine;
using System.Collections;

public class MovingCamera : MonoBehaviour {
    public GameObject m_lookAt; // GameObject with a position which camera will be at 
    public float m_moveSpeed = 0.1f; // Speed at which camera will move
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = Vector3.Lerp(transform.position, m_lookAt.transform.position, m_moveSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, m_lookAt.transform.rotation, m_moveSpeed);
	}
}
