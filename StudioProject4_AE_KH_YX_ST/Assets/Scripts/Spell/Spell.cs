using UnityEngine;
using System.Collections;

public class Spell : MonoBehaviour {
    [HideInInspector]
    public Timer effectTimer;
    public float effectTime;
    public float timer;
    public Vector3 m_targetPos;
	// Use this for initialization
	void Start () {
        effectTimer = gameObject.AddComponent<Timer>();
        effectTimer.Init(0, effectTime, 0);
        transform.position = new Vector3(LevelManager.instance.PlayerBase.transform.position.x, LevelManager.instance.PlayerBase.transform.position.y, LevelManager.instance.PlayerBase.transform.position.z);
        CardProjectile m_projectile = gameObject.AddComponent<CardProjectile>();
        m_projectile.target = new GameObject("projectile"); 
        m_projectile.target.transform.position = new Vector3(m_targetPos.x, m_targetPos.y, m_targetPos.z);
        timer = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
        effectTimer.Update();
        if (gameObject.transform.GetChild(1).gameObject.activeSelf)
        {
            SceneData.sceneData.is_spellHit = true;
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
            gameObject.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>().Stop();
            timer += Time.deltaTime;
        }
        if (gameObject.transform.GetChild(1).gameObject.activeSelf /*&& gameObject.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>().isStopped*/ && timer > 0.3f)
        {
            SceneData.sceneData.is_spellCast = false;
            Destroy(this.gameObject);
            Destroy(this.GetComponent<Spell>());
            Destroy(this.GetComponent<CardProjectile>());
            SceneData.sceneData.is_spellHit = false;
        }
	}
}
