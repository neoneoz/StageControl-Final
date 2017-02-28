using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExplosionManager : MonoBehaviour {
    public static ExplosionManager instance = null;
    public ParticleSystem ExplosionParticle = null;
    List<ParticleSystem> Explosions = new List<ParticleSystem>();

	// Use this for initialization
	void Start ()
    {
        if (instance == null)
        {
            instance = this;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        List<ParticleSystem> ToBeRemoved = new List<ParticleSystem>();
        foreach (ParticleSystem explosion in Explosions)
        {
            if (explosion.time >= 2)
            {
                ToBeRemoved.Add(explosion);
            }
        }

        while(ToBeRemoved.Count > 0)
        {
            ParticleSystem temp = ToBeRemoved[0];
            Explosions.Remove(temp);
            ToBeRemoved.Remove(temp);
            Destroy(temp);
        }
	}

    public void ApplyExplosion(Vector3 position)
    {
        ParticleSystem temp = Instantiate(ExplosionParticle);
        temp.transform.position = position;
        if (temp != null)
        {
            Explosions.Add(temp);

            temp.Play();
        }
        //explosionTimer += Time.deltaTime;
        //if (explosionTimer < 2)
        //{
        //    Explosions.Play();
   
        //    //Explosions.transform.position = gameObject.GetComponent<Building>().transform.position;
        //    Explosions.transform.position = gameObject.transform.position;
        //}

        //if (explosionTimer >= 2)
        //{
        //    Explosions.Stop();
        //    Destroy(Explosions);
        //}
    }
}
