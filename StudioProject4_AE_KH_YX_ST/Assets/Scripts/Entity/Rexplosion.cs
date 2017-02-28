using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rexplosion : MonoBehaviour {

    public bool exploded = false , m_isfriendly;
    public float damage,time = 0, radius = 2;
    public List<GameObject> nearbyList;
	// Use this for initialization
	void Start () {
        //target = null;
       
	}
	public void setExplosion(float dmg)
    {
        damage = dmg;
        //m_isfriendly = friendly;
    }

	// Update is called once per frame
	void Update () {
       time += Time.deltaTime; 
        if(!exploded)
        {
            // See unit.cs if there are enquiries bout the code below
            nearbyList = SpatialPartition.instance.GetObjectListAt(transform.position);
            //List<GameObject> nearbyList = Spawn.m_entityList;
            //nearbyList.AddRange(Building.m_buildingList);
            for (int i = 0; i < nearbyList.Count; ++i)//SCROLL THRU ALL ENTETIES
            { 
                GameObject ent = nearbyList[i];
                //if (ent.GetComponent<Unit>().m_isFriendly == m_isfriendly) // if is same team , ignore
                    //continue;

                float dist = (ent.transform.position - transform.position).sqrMagnitude;
                if (dist <= radius * radius) // An enemy has drawn close to the unit, attack it
                    DoDamage(ent);
            }
            PlayAudio.instance.m_source.clip = PlayAudio.instance.m_railgunExplode;
            PlayAudio.instance.m_source.volume = 0.7f;
            PlayAudio.instance.PlayOnce();
            exploded = true;
        }
        if (time > 1.3)
            Destroy(gameObject);
	
	}

    void DoDamage(GameObject target)
    {
        if (target.GetComponent<Unit>() != null)
            target.GetComponent<Unit>().TakeDamage(damage);
        else
            target.GetComponent<Building>().TakeDamage(damage);
    }
}
