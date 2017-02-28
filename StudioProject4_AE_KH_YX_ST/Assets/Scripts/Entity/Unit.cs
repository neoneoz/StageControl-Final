using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum UNIT_TEAM
{
    BLUE_TEAM,
    RED_TEAM,
}

/**/
// Controller class of all units
/**/
[RequireComponent(typeof(Flocking))]
[RequireComponent(typeof(VMovement))]
public class Unit : MonoBehaviour
{
    public enum UNIT_TYPE
    {
        CW_KNIGHT,
        BALLISTA,
        SPIDER_TNK,
        BBUSTER,
        RAILGUN,
        IEN_GOLEM

    };



    public UNIT_TYPE m_type;
    public int m_attkRadius; // Attack radius which is how many grids surrounding the unit it can detect/see enemies
    public int m_attkDamage;
    private float m_attkDist; // Minimum distance before unit attacks closest enemy unit
    private Vector2 m_currGrid; // The current grid the unit is standing on
    private Vector2 m_oldGrid; // The grid the unit was standing on in the previous frame

    public AnimationClip m_attkAnim; // The attack animation
    public float m_attackspeed;//time between attacks

    public float m_health;
    public float m_maxHealth;
    private Timer m_timer;
    public static Component[] m_destroyerOfWorlds; // Stores and destroys all the components of an object

    public List<Vector3> PathToEnd = null;
    public int pathindex = 0;
    private UNIT_TEAM m_team; // The team this entity belongs to
    public bool m_isFriendly; // is the unit player or enemy team's?
    public GameObject m_targetEnemy; // The unit this unit is trying to kill
    private List<GameObject> m_targetList; // Queue data structure containing buildings or units that unit will attack
    private int m_targetIndex; // The current target in the list unit is focusing attack on
    [HideInInspector]
    public Building m_building; // The building which spawned this unit

    //attack stuff
    public GameObject projectile;
    public GameObject Emitter;//for hitscan units
    public bool ismelee;

    //Health stuff
    Image friendlyHealth = null;
    Image enemyHealth = null;

    public void SetPath(List<Vector3> newPath)
    {
        PathToEnd = newPath;
        pathindex = newPath.Count - 1;
    }

    // Use this for initialization
    void Start()
    {
        Invoke("InstantiateStats", 0.1f);

        m_currGrid = SceneData.sceneData.gridmesh.GetGridIndexAtPosition(transform.position);
        m_oldGrid = SceneData.sceneData.gridmesh.GetGridIndexAtPosition(transform.position);

        Vector2 temp1 = m_currGrid;
        int x = (int)m_currGrid.x;
        int z = (int)m_currGrid.y;
        for (int i = x - m_attkRadius; i <= x + m_attkRadius; ++i) // Print out the attack radius which is a ring around the unit
        {
            for (int j = z - m_attkRadius; j <= z + m_attkRadius; ++j)
            {
                temp1.x = i;
                temp1.y = j;
                //SceneData.sceneData.gridmesh.HighlightUnitPosition(temp1);
            }
        }

        Vector2 farGrid; // Stores furthest grid unit can detect enemy 
        farGrid.x = m_currGrid.x + m_attkRadius; // The grid which unit is on is offset by number of grids away which he can sense so we can get furthest away grid
        farGrid.y = m_currGrid.y;
        Vector3 furthestPoint = SceneData.sceneData.gridmesh.GetPositionAtGrid((int)farGrid.x, (int)farGrid.y); // Get the position of the furthest away grid
        m_attkDist = (furthestPoint - transform.position).sqrMagnitude; // sqrmagnitude is less expensive since we are just doing distance checks//what does this vairable do



        m_timer = this.gameObject.AddComponent<Timer>();
        //m_timer.Init(0, m_attkAnim.length * 2, m_attkAnim.length * 2);

        m_timer.Init(0, m_attackspeed, 0);
        //the timer runs for the attack speed tiem


        m_timer.can_run = true;
        m_destroyerOfWorlds = new Component[100];
        m_targetEnemy = null;
        //m_targetBuilding = null;
        m_targetList = new List<GameObject>();
        m_targetIndex = 0;

        if (m_type == UNIT_TYPE.SPIDER_TNK || m_type == UNIT_TYPE.BBUSTER || m_type == UNIT_TYPE.RAILGUN)
        {
            Emitter = Instantiate(Emitter);
            Emitter.SetActive(false);
        }
    }

    void InstantiateStats()
    {
        friendlyHealth = Instantiate(SceneData.sceneData.Health_friendly);
        friendlyHealth.transform.SetParent(SceneData.sceneData.UI.transform);
        friendlyHealth.enabled = false;
        friendlyHealth.transform.GetChild(0).GetComponent<Image>().enabled = false;

        enemyHealth = Instantiate(SceneData.sceneData.Health_enemy);
        enemyHealth.transform.SetParent(SceneData.sceneData.UI.transform);
        enemyHealth.enabled = false;
        enemyHealth.transform.GetChild(0).GetComponent<Image>().enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_currGrid = SceneData.sceneData.gridmesh.GetGridIndexAtPosition(transform.position); // Get unit's current grid

        //check if target is alive


        if (m_targetEnemy == null)
        {
            //projectile.SetActive(false);
            if(Emitter)
            Emitter.SetActive(false);


            //rotate the unit properly
            Animator anim = gameObject.transform.GetChild(0).GetComponent<Animator>();
            anim.SetBool("b_attack", false);
            GetComponent<VMovement>().m_stopMove = false;
            //Mathf.Rad2Deg();
            float rotation = (Mathf.Atan2(gameObject.GetComponent<VMovement>().Velocity.z, gameObject.GetComponent<VMovement>().Velocity.x));
            gameObject.transform.rotation = new Quaternion(0, 1, 0, rotation + 1.5708f);

            for (int i = 0; i < Spawn.m_entityList.Count; ++i)//SCROLL THRU ALL ENTETIES
            {
                GameObject ent = (GameObject)Spawn.m_entityList[i];

                if (ent.GetComponent<Unit>().m_isFriendly == m_isFriendly) // if is same team , ignore
                    continue;

                float dist = (ent.transform.position - transform.position).sqrMagnitude;
                if (dist <= m_attkRadius * m_attkRadius) // An enemy has drawn close to the unit, attack it
                    m_targetEnemy = ent;
            }

            if (m_targetEnemy == null) // still no enemy found
            {
                /**/
                for (int i = 0; i < Building.m_buildingList.Count; ++i)
                {
                    GameObject ent = Building.m_buildingList[i];
                    if (ent.GetComponent<Building>().isfriendly == m_isFriendly) // if not from the same team
                        continue;
                    float dist = (ent.transform.position - transform.position).sqrMagnitude;

                    if (dist <= m_attkRadius * m_attkRadius) // distance check to see if building to attack is nearby
                        m_targetEnemy = ent;

                    }
                

            }

        }


        if (m_targetEnemy)
            DoAttack();



     
            if (PathToEnd.Count > 0)
            {
                GetComponent<VMovement>().Velocity = (PathToEnd[pathindex] - transform.position).normalized;
                if ((PathToEnd[pathindex] - transform.position).sqrMagnitude < GetComponent<VMovement>().speed * GetComponent<VMovement>().speed)
                {
                    --pathindex;
                    if (pathindex <= 0)
                    {
                        PathToEnd.Clear();
                    }
                }
            }
        

        if (m_isFriendly == true && friendlyHealth != null)
        {
            friendlyHealth.enabled = true;
            friendlyHealth.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position + gameObject.transform.up.normalized * 10);

            friendlyHealth.transform.GetChild(0).GetComponent<Image>().enabled = true;
            friendlyHealth.transform.GetChild(0).GetComponent<Image>().fillAmount = m_health / m_maxHealth;
            friendlyHealth.transform.GetChild(0).GetComponent<Image>().transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position + gameObject.transform.up.normalized * 10);
        }

        else if (m_isFriendly == false && enemyHealth != null)
        {
            enemyHealth.enabled = true;
            enemyHealth.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position + gameObject.transform.up.normalized * 10);

            enemyHealth.transform.GetChild(0).GetComponent<Image>().enabled = true;
            enemyHealth.transform.GetChild(0).GetComponent<Image>().fillAmount = m_health / m_maxHealth;
            enemyHealth.transform.GetChild(0).GetComponent<Image>().transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position + gameObject.transform.up.normalized * 10);
        }
        /*Affected by spell*/
        if (SceneData.sceneData.is_spellCast && SceneData.sceneData.is_spellHit)
        {
            if (SceneData.sceneData.gridmesh.CheckWithinRadius(transform.position, SceneData.sceneData.spell_grid))
            {
                m_health -= Time.deltaTime * (int)SceneData.sceneData.spell_dmg;
                //GetComponent<Health>().DecreaseHealthGradually(Time.deltaTime, (int)SceneData.sceneData.spell_dmg);
            }
        }
       

        //if ((m_targetBuilding == null || m_targetBuilding.GetComponent<Health>().GetHealth() < 0) && GetComponent<VMovement>().m_stopMove2)
        //{
        //    m_targetBuilding = null;
        //    GetComponent<VMovement>().m_stopMove2 = false;
        //}
        //else if (m_targetBuilding != null && m_building)
        //{
        //    m_building.m_isDistract = true;
        //    m_building.GetComponent<Pathfinder>().FindPath(m_building.GetMaxPosOfBuilding(m_targetBuilding.transform.position, m_targetBuilding.GetComponent<Building>().size));
        //    SetPath(m_building.GetComponent<Pathfinder>().PathToEnd);
        //    Debug.Log("decreasing");
        //    m_targetBuilding.GetComponent<Health>().DecreaseHealthGradually(Time.deltaTime, m_attkDamage);
        //    GetComponent<VMovement>().m_stopMove2 = true;
        //    if (m_targetBuilding.GetComponent<Health>().GetHealth() < 0)
        //        m_building.m_isDistract = false;
        //}

        if (m_health <= 0) // Unit died
        {
            if (Emitter != null)
                Destroy(Emitter);
            RemoveEntity(this.gameObject);
            UnityEngine.Object.Destroy(this.gameObject);
            m_destroyerOfWorlds = GetComponents(typeof(Component));
            for (int i = 0; i < m_destroyerOfWorlds.Length; ++i)
            {
                if (m_destroyerOfWorlds[i].gameObject.activeSelf)
                    UnityEngine.Object.Destroy(m_destroyerOfWorlds[i]);
            }
            Destroy(friendlyHealth);
            Destroy(enemyHealth);
        }


    }

    public void TakeDmage(float damage)
    {
        m_health -= damage;

       
 

    }


    void DoAttack()
    {
        m_timer.Update();//update attack speed
        if (GetHealth() <=0 )//enemy hp check
        {
            m_targetEnemy = null;
            return;
        }

        Vector3 displacement = m_targetEnemy.transform.position - transform.position;
        if (!ismelee && (displacement).sqrMagnitude > m_attkRadius * m_attkRadius)//range check
        {
            m_targetEnemy = null;
            return;
        }
        else if (ismelee)
        {
            float mrange = 60;
            if (ismelee && displacement.sqrMagnitude > mrange)
            {
                gameObject.GetComponent<VMovement>().Velocity = displacement.normalized * 35;
                GetComponent<VMovement>().m_stopMove = false;
                return;
            }
        }




        if (!m_timer.can_run)//atkspeed check
            return;
        if (!ismelee)
            GetComponent<VMovement>().m_stopMove = true;//stop moving to whack


        //Attack sucess past this line//////////////////////////
        Animator anim = gameObject.transform.GetChild(0).GetComponent<Animator>();
        anim.SetBool("b_attack", true);
        m_timer.Reset();//reset atk speed timer
        switch (m_type)
        {
            case (UNIT_TYPE.CW_KNIGHT):
                {
                    //probably change later
                    GetComponent<VMovement>().m_stopMove = true;
                    DoDamage(m_attkDamage);
                    //do damage

                } break;

            case (UNIT_TYPE.BALLISTA):
                {
                    //Instantiate()
                    GameObject bullet = Instantiate(projectile, gameObject.transform.position, Quaternion.identity) as GameObject;
                    bullet.GetComponent<Bprojectile>().setprojectile(m_targetEnemy, m_attkDamage, 1);
                } break;

            case (UNIT_TYPE.SPIDER_TNK):
                {
                    //GameObject flash = Instantiate(projectile, gameObject.transform.GetChild(1).transform.position, Quaternion.identity) as GameObject;
                    Emitter.SetActive(true);
                    Emitter.transform.position = gameObject.transform.GetChild(1).transform.position;
                    DoDamage(m_attkDamage);

                } break;
            case (UNIT_TYPE.BBUSTER):
                {
                    Emitter.SetActive(true);
                    Emitter.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y+10, gameObject.transform.position.z);
                    //Emitter.transform.position.y += 20;
                    DoDamage(m_attkDamage);
                } break;
            case (UNIT_TYPE.RAILGUN):
                {
                    Emitter.SetActive(true);
                    Emitter.transform.position = gameObject.transform.GetChild(1).transform.position;
                    Vector3 pos = new Vector3(m_targetEnemy.transform.position.x, m_targetEnemy.transform.position.y + 1000, m_targetEnemy.transform.position.z);

                    GameObject blast = Instantiate(projectile, pos, Quaternion.identity) as GameObject;
                    blast.GetComponent<Bprojectile>().setprojectile(m_targetEnemy, m_attkDamage, 2, 400);

                } break;
            case (UNIT_TYPE.IEN_GOLEM):
                {
                    //probably change later
                    GetComponent<VMovement>().m_stopMove = true;
                    DoDamage(m_attkDamage);
                    //do damage
                } break;

        }






    }

    void DoDamage(float damage)
    {
        if( m_targetEnemy.GetComponent<Unit>() != null)
            m_targetEnemy.GetComponent<Unit>().TakeDamage(m_attkDamage);
        else
            m_targetEnemy.GetComponent<Building>().TakeDamage(m_attkDamage);

    }

    public void TakeDamage(float damage)
    {
        m_health -= damage;
    }

    float GetHealth()
    {
        if(m_targetEnemy.GetComponent<Unit>() != null)
            return m_targetEnemy.GetComponent<Unit>().m_health;
        else
            return m_targetEnemy.GetComponent<Building>().buildingHealth;
    }

    // Remove a gameobject from Spawn.m_entityList
    public static void RemoveEntity(GameObject go, List<GameObject> list = null)
    {
        if (list == null)
            Spawn.m_entityList.Remove(go);
        else
            list.Remove(go);
    }

}






