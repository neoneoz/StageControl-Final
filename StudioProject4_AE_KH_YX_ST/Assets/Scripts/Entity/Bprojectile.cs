using UnityEngine;
using System.Collections;

public class Bprojectile : MonoBehaviour {

    public GameObject target,explosion ;
    Vector3 Velocity;
    public float damage;
    public float speed;
    int type;
	// Use this for initialization
	void Start () {
        //target = null;
       
	}
	public void setprojectile(GameObject unit, float dmg,int ptype)
    {
        target = unit;
        damage = dmg;
        type = ptype;
    }
    public void setprojectile(GameObject unit, float dmg, int ptype,float spd)
    {
        target = unit;
        damage = dmg;
        type = ptype;
        speed = spd;
    }
	// Update is called once per frame
	void Update () {
        if (target == null)
            return;
        
        Vector3 displacement = (target.transform.position - gameObject.transform.position);
      
        switch (type)
        {
        case(1):
        Velocity = displacement.normalized * speed;
        if(displacement.sqrMagnitude < 10f*10f)
        {
            target.GetComponent<Unit>().TakeDmage(damage);
            DestroyObject(gameObject);
        }break;
        case(2):


        Velocity = new Vector3(0, -1, 0) * speed; 
        if(transform.position.y < -200)
        { 
            Instantiate(explosion, target.transform.position, Quaternion.identity);
            DestroyObject(gameObject);
            

        }
        break;
          
  
    }
        gameObject.transform.position += Velocity * Time.deltaTime;
        
	
	}
}
