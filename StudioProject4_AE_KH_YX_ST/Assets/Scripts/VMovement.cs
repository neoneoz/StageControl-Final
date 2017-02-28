using UnityEngine;
using System.Collections;

public class VMovement : MonoBehaviour
{
    Vector3 Goal;
    public Vector3 Velocity;
    public float speed = 10f;
    Vector3 nextpos = new Vector3();
    Grid nextgrid;
    bool moveX = true;
    bool moveZ = true;
    public bool m_stopMove;
    //public bool m_stopMove2;
	// Use this for initialization
	void Start () 
    {
        Velocity.Set(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        m_stopMove = false;
       // m_stopMove2 = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!m_stopMove)
        {

            transform.position = transform.position + Velocity.normalized * speed * Time.deltaTime;
        }
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, SceneData.sceneData.ground.SampleHeight(gameObject.transform.position), gameObject.transform.position.z);


        moveX = true;
        moveZ = true;
	}
}
