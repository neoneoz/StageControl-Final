using UnityEngine;
using System.Collections;

public class Spell : MonoBehaviour {
    [HideInInspector]
    public Timer effectTimer;
    public float effectTime;
    public Vector3 m_targetPos;
	// Use this for initialization
	void Start () {
        effectTimer = gameObject.AddComponent<Timer>();
        effectTimer.Init(0, effectTime, 0);
        transform.position = new Vector3(LevelManager.instance.PlayerBase.transform.position.x, LevelManager.instance.PlayerBase.transform.position.y, LevelManager.instance.PlayerBase.transform.position.z);
        Bprojectile m_projectile = gameObject.AddComponent<Bprojectile>();
        m_projectile.target = new GameObject("projectile"); 
        m_projectile.target.transform.position = new Vector3(m_targetPos.x, m_targetPos.y, m_targetPos.z);
	}
	
	// Update is called once per frame
	void Update () {
        effectTimer.Update();
        if (effectTimer.can_run)
        {
            SceneData.sceneData.is_spellCast = false;
            //Destroy(this.gameObject);
            //Destroy(this.GetComponent<Spell>());
        }
	}
}
