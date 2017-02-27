using UnityEngine;
using System.Collections;

public class TriggerAnimation : MonoBehaviour {
    Timer m_animTimer;
    bool m_changeAnim;
    Animator anim;
    public GameObject m_fx;
    ParticleSystem ps;
	// Use this for initialization
	void Start () {
        m_animTimer = gameObject.AddComponent<Timer>();
        m_animTimer.Init(0, 10, 0);
        m_changeAnim = false;
        anim = GetComponent<Animator>();
        ps = m_fx.GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        m_animTimer.Update();
        if (m_animTimer.can_run)
        {
            anim.SetBool("b_attack", true);
            m_changeAnim = true;
            m_animTimer.Reset();
            if (!ps.isPlaying)
            {
                ps.gameObject.SetActive(true);
            }
        }
        else if (m_changeAnim)
        {
            if (ps.gameObject.activeSelf && ps.isStopped)
            {
                ps.Stop();
                ps.gameObject.SetActive(false);
            }
            anim.SetBool("b_attack", false);
            m_changeAnim = false;
        }        
        //if (!ps.IsAlive())
        //{
        //    Debug.Log("ok");
        //    ps.gameObject.SetActive(false);
        //}
    }
}
