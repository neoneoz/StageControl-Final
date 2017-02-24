using UnityEngine;
using System.Collections;

public class Bprojectile : MonoBehaviour {

    public GameObject target ;
    Vector3 Velocity;
    public float damage;
    public float speed;
	// Use this for initialization
	void Start () {
        //target = null;
       
	}
	public void setprojectile(GameObject unit, float dmg)
    {
        target = unit;
        damage = dmg;
    }
	// Update is called once per frame
	void Update () {
        if (target == null)
            return;

        Vector3 displacement = (target.transform.position - gameObject.transform.position);
        Velocity = displacement.normalized * speed;
        if(displacement.sqrMagnitude < 10f*10f)
        {
            //target.GetComponent<Unit>().m_health -= damage;//take damage code
            DestroyObject(gameObject);
        }

        gameObject.transform.position += Velocity * Time.deltaTime;
        
	
	}
}
