using UnityEngine;
using System.Collections;

public class Bprojectile : MonoBehaviour {

    //public GameObject projectileObj; // Object representation of projectile
    public GameObject target; // Where/which projectile aims to go to
    Vector3 Velocity;
    public float damage;
    public float speed;
    private float prev_speed; // The speed from the frame before
    private float trajectoryHeight; // The height at which the projectile can reach
    private Quaternion lookRotation; // Which direction should projectile look
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
        Vector3 dir = displacement.normalized;
        trajectoryHeight = displacement.magnitude /*+ SceneData.sceneData.ground.SampleHeight(transform.position)*/;
        if (GetComponent<Spell>() != null)
        {
            speed = (prev_speed + (displacement / GetComponent<Spell>().effectTime).magnitude) * 0.5f;
            prev_speed = speed;
            //transform.GetChild(1).gameObject.SetActive(true);
        }
        Velocity = displacement.normalized * speed;

        if(displacement.sqrMagnitude < 10f*10f && GetComponent<Spell>().effectTimer.can_run)
        {
            //target.GetComponent<Unit>().m_health -= damage;//take damage code
            DestroyObject(gameObject);
            DestroyObject(target);
        }

        gameObject.transform.position += Velocity * Time.deltaTime;
        // Arc
        if (GetComponent<Spell>() != null)
        {
            float cTime = displacement.magnitude;
            // calculate straight-line lerp position:
            Vector3 currentPos = Vector3.Lerp(LevelManager.instance.PlayerBase.transform.position, target.transform.position, cTime);
            // add a value to Y, using Sine to give a curved trajectory in the Y direction
            transform.position = new Vector3(transform.position.x, transform.position.y + (trajectoryHeight * Mathf.Sin(Mathf.Clamp01(cTime) * Mathf.PI)), transform.position.z);
            //create the rotation we need to be in to look at the target
            lookRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
        }
	}
}
