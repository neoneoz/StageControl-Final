using UnityEngine;
using System.Collections;

public class CardProjectile : MonoBehaviour
{

    //public GameObject projectileObj; // Object representation of projectile
    public GameObject target; // Where/which projectile aims to go to
    Vector3 Velocity;
    public float damage;
    public float speed;
    private float prev_speed;
    private float trajectoryHeight; // The height at which the projectile can reach
    private Quaternion lookRotation; // Which direction should projectile look
    private float original_magnitude; // The original displacement or distance between the projectile and its destination
    // Use this for initialization
    void Start()
    {
        original_magnitude = 0;
    }
    public void setprojectile(GameObject unit, float dmg)
    {
        target = unit;
        damage = dmg;
    }
    // Update is called once per frame
    void Update()
    {
        if (target == null)
            return;
        if (original_magnitude == 0)
        {
            original_magnitude = (target.transform.position - gameObject.transform.position).magnitude;
            speed = original_magnitude / GetComponent<Spell>().effectTime;
        }
        Vector3 displacement = (target.transform.position - gameObject.transform.position);
        Vector3 dir = displacement.normalized;
        trajectoryHeight = SceneData.sceneData.ground.SampleHeight(transform.position);
        Velocity = displacement.normalized * speed;
        prev_speed = displacement.magnitude / GetComponent<Spell>().effectTime;
        if (displacement.sqrMagnitude < 10 * 10 && GetComponent<Spell>().effectTimer.can_run)
        {
            ParticleSystem ps = gameObject.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>();
            if (!ps.isPlaying)
            {
                ps.transform.position = new Vector3(target.transform.position.x, SceneData.sceneData.ground.SampleHeight(transform.position) + 20, target.transform.position.z);
                ps.gameObject.SetActive(true);
            }
        }

        gameObject.transform.position += Velocity * Time.deltaTime;
        // Arc
        float cTime = displacement.magnitude;
        // calculate straight-line lerp position:
        Vector3 currentPos = Vector3.Lerp(LevelManager.instance.PlayerBase.transform.position, target.transform.position, cTime);
        // add a value to Y, using Sine to give a curved trajectory in the Y direction
        transform.position = new Vector3(transform.position.x, prev_speed + currentPos.y + (SceneData.sceneData.ground.terrainData.size.y * Mathf.Sin(Mathf.Clamp01(cTime) * Mathf.PI)), transform.position.z);
        //create the rotation we need to be in to look at the target
        lookRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
    }

    void OnDestroy()
    {
        DestroyObject(target);
    }
}
