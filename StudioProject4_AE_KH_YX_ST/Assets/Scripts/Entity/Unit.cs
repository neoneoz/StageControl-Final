using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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
        IEN_GOLEM,
        MAX_TYPE
    };

    public UNIT_TYPE m_type; // Which category/species does unit belong to
    public int m_attkRadius; // Attack radius which is how many grids surrounding the unit it can detect/see enemies
    public int m_attkDamage; // How much does this unit damage other's hitpoints
    private float m_attkDist; // Minimum distance before unit attacks closest enemy unit
    private Vector2 m_currGrid; // The current grid the unit is standing on
    private Vector2 m_oldGrid; // The grid the unit was standing on in the previous frame

    public AnimationClip m_attkAnim; // The attack animation
    public float m_attackspeed;//time between attacks

    public float m_health;
    public float m_maxHealth;
    private Timer m_timer; // Cooldown for unit's attacks
    public List<Vector3> PathToEnd = null; // Pathfinding list of calculated waypoints to follow, a* method
    public int pathindex = 0;
    public bool m_isFriendly; // is the unit player or enemy team's?
    public GameObject m_targetEnemy; // The unit this unit is trying to kill
    [HideInInspector]
    public Building m_building; // The building which spawned this unit

    //attack stuff
    public GameObject projectile;
    public GameObject Emitter;//for hitscan units
    public bool ismelee; // checks if this a range or melee unit

    //SP, distance checking to attack
    public List<GameObject> nearbyList;
    public int minindex_x;
    public int minindex_y;
    public int maxindex_x;
    public int maxindex_y;

    //Healthbar stuff
    Image friendlyHealth = null;
    Image enemyHealth = null;

    //ID
    uint ID;

    // Audio check variables
    float m_volume;
    static bool[] m_audioList; // Contains all the entities playing the music

    public uint GetID()
    {
        return ID;
    }

    // Gold carried by each unit
    public int m_gold;
    private PlayerInfo m_player; // The instance to the player, could be singleton then dont need a reference


    public void SetPath(List<Vector3> newPath)
    {
        PathToEnd = newPath;
        pathindex = newPath.Count - 1;
    }

    // Use this for initialization
    void Start()
    {
        Invoke("InstantiateStats", 0.1f);

        // Get grid unit spawn on
        m_currGrid = SceneData.sceneData.gridmesh.GetGridIndexAtPosition(transform.position);
        m_oldGrid = SceneData.sceneData.gridmesh.GetGridIndexAtPosition(transform.position);

        Vector2 farGrid; // Stores furthest grid that unit can detect enemy 
        farGrid.x = m_currGrid.x + m_attkRadius; // The grid which unit is on is offset by number of grids away which he can sense so we can get furthest away grid
        farGrid.y = m_currGrid.y;
        Vector3 furthestPoint = SceneData.sceneData.gridmesh.GetPositionAtGrid((int)farGrid.x, (int)farGrid.y); // Get the position of the furthest away grid
        m_attkDist = (furthestPoint - transform.position).sqrMagnitude; // sqrmagnitude is less expensive since we are just doing distance checks//what does this vairable do

        m_timer = this.gameObject.AddComponent<Timer>();
        //m_timer.Init(0, m_attkAnim.length * 2, m_attkAnim.length * 2); //Uses attack animation time, no longer used

        //the timer runs for the attack speed time        
        m_timer.Init(0, m_attackspeed, 0);

        m_timer.can_run = true; // Let them attack immediately at first
        m_targetEnemy = null; // set to null so program know to assign enemies to this variable later

        // If they are ranged units
        if (m_type == UNIT_TYPE.SPIDER_TNK || m_type == UNIT_TYPE.BBUSTER || m_type == UNIT_TYPE.RAILGUN)
        {
            Emitter = Instantiate(Emitter);
            Emitter.SetActive(false);
        }

        ID = SceneData.sceneData.GetUniqueID();
        // The player object
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInfo>();
        m_audioList = new bool[(int)UNIT_TYPE.MAX_TYPE]; // Contains all the entities playing the music
    }

    // For init'ing healthbar status values and add entity to Spatial partioning
    void InstantiateStats()
    {
        SpatialPartition.instance.AddGameObject(gameObject);
        friendlyHealth = Instantiate(SceneData.sceneData.Health_friendly);
        friendlyHealth.transform.SetParent(SceneData.sceneData.UI.transform);
        friendlyHealth.enabled = false;
        friendlyHealth.transform.GetChild(0).GetComponent<Image>().enabled = false;

        enemyHealth = Instantiate(SceneData.sceneData.Health_enemy);
        enemyHealth.transform.SetParent(SceneData.sceneData.UI.transform);
        enemyHealth.enabled = false;
        enemyHealth.transform.GetChild(0).GetComponent<Image>().enabled = false;
    }

    // Find enemy on same spatial partioning grid, which is less cost-expensive, better than a for loop through all entities in whole scene with a distance check inside
    void FindTarget()
    {
        nearbyList = SpatialPartition.instance.GetObjectListAt(transform.position, m_attkRadius, out minindex_x, out minindex_y, out maxindex_x, out maxindex_y);

        if (m_targetEnemy == null) // no enemy targetted
        {
            //projectile.SetActive(false);
            if (Emitter)
                Emitter.SetActive(false);


            //rotate the unit properly towards objective, for example an enemy
            Animator anim = gameObject.transform.GetChild(0).GetComponent<Animator>();
            anim.SetBool("b_attack", false);
            GetComponent<VMovement>().m_stopMove = false;
            //Mathf.Rad2Deg();
            float rotation = (Mathf.Atan2(gameObject.GetComponent<VMovement>().Velocity.z, gameObject.GetComponent<VMovement>().Velocity.x));
            gameObject.transform.rotation = new Quaternion(0, 1, 0, rotation + 1.5708f);

            //for (int i = 0; i < Spawn.m_entityList.Count; ++i)//SCROLL THRU ALL ENTETIES
            //{
            //    //if (!nearbyList[i] || nearbyList[i].GetComponent<Building>())
            //    //    continue;
            //    GameObject ent = Spawn.m_entityList[i];

            //    if (ent.GetComponent<Unit>().m_isFriendly == m_isFriendly) // if is same team , ignore
            //        continue;

            //    float dist = (ent.transform.position - transform.position).sqrMagnitude;
            //    if (dist <= m_attkRadius * m_attkRadius) // An enemy has drawn close to the unit, attack it
            //        m_targetEnemy = ent;
            //}

            //if (m_targetEnemy == null) // still no enemy found
            //{
            //    /**/
            //    for (int i = 0; i < Building.m_buildingList.Count; ++i)
            //    {
            //        //if (!nearbyList[i] || nearbyList[i].GetComponent<Unit>())
            //        //    continue;
            //        GameObject ent = Building.m_buildingList[i];
            //        if (ent.GetComponent<Building>().isfriendly == m_isFriendly) // if not from the same team
            //            continue;
            //        float dist = (ent.transform.position - transform.position).sqrMagnitude;

            //        if (dist <= m_attkRadius * m_attkRadius) // distance check to see if building to attack is nearby
            //            m_targetEnemy = ent;

            //    }
            //}

            for (int i = 0; i < nearbyList.Count; ++i)//SCROLL THRU ALL ENTETIES
            {
                if (!nearbyList[i] || nearbyList[i].GetComponent<Building>()) // If object is null or is a building, skip dat
                    continue;
                GameObject ent = nearbyList[i]; 

                if (ent.GetComponent<Unit>().m_isFriendly == m_isFriendly) // if is same team , ignore
                    continue;

                float dist = (ent.transform.position - transform.position).sqrMagnitude; // distance check, should be close enough for attacking
                if (dist <= m_attkRadius * m_attkRadius) // An enemy has drawn close to the unit, attack it
                    m_targetEnemy = ent; // assigned enemy
            }

            if (m_targetEnemy == null) // still no enemy found, time to find buildings instead, so priority is actually units then buildings
            {
                /**/
                for (int i = 0; i < nearbyList.Count; ++i)
                {
                    if (!nearbyList[i] || nearbyList[i].GetComponent<Unit>()) // if not a unit
                        continue;
                    if(nearbyList[i].GetComponent<Building>().b_state != Building.BUILDSTATE.B_ACTIVE) // It should be active and not dead
                        continue;

                    GameObject ent = nearbyList[i];
                    if (ent.GetComponent<Building>().isfriendly == m_isFriendly) // if not from the same team
                        continue;
                    float dist = (ent.transform.position - transform.position).sqrMagnitude;

                    if (dist <= m_attkRadius * m_attkRadius) // distance check to see if building to attack is nearby
                        m_targetEnemy = ent;

                }
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_currGrid = SceneData.sceneData.gridmesh.GetGridIndexAtPosition(transform.position); // Get unit's current grid

        // Gets enemy to target
        FindTarget();

        //check if target is alive,
        if (m_targetEnemy)
            DoAttack();

        if (PathToEnd.Count > 0) // If there are places to go, go
        {
            GetComponent<VMovement>().Velocity = (PathToEnd[pathindex] - transform.position).normalized; // direction vector
            if ((PathToEnd[pathindex] - transform.position).sqrMagnitude < GetComponent<VMovement>().speed * GetComponent<VMovement>().speed)
            {
                --pathindex;
                if (pathindex <= 0)
                {
                    PathToEnd.Clear();
                }
            }
        }
        
        // Make a friendly healthbar if unit is on the player's side
        if (m_isFriendly == true && friendlyHealth != null)
        {
            friendlyHealth.enabled = true;
            friendlyHealth.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position + gameObject.transform.up.normalized * 10); // Get screen coordinates so the healthbar is not a world space majinky
            friendlyHealth.transform.GetChild(0).GetComponent<Image>().enabled = true; // Set its healthbar active
            friendlyHealth.transform.GetChild(0).GetComponent<Image>().fillAmount = m_health / m_maxHealth; // Healthbar ratio of current health to max
            friendlyHealth.transform.GetChild(0).GetComponent<Image>().transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position + gameObject.transform.up.normalized * 10); // Position bar above entity's head however if entity is too high up, the x10 would cause bar to reach for the stars
        }
        // Make a enemy healthbar if unit is on the AI/enemy's side
        else if (m_isFriendly == false && enemyHealth != null)
        {
            enemyHealth.enabled = true;
            enemyHealth.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position + gameObject.transform.up.normalized * 10);

            enemyHealth.transform.GetChild(0).GetComponent<Image>().enabled = true;
            enemyHealth.transform.GetChild(0).GetComponent<Image>().fillAmount = m_health / m_maxHealth;
            enemyHealth.transform.GetChild(0).GetComponent<Image>().transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position + gameObject.transform.up.normalized * 10);
        }
        /*Affected by spell*/
        if (SceneData.sceneData.is_spellCast && SceneData.sceneData.is_spellHit) // if spell is cast and its projectile has reached the ground
        {
            if (SceneData.sceneData.gridmesh.CheckWithinRadius(transform.position, SceneData.sceneData.spell_grid)) // check if i as the unit am in range of blast
            {
                m_health -= Time.deltaTime * (int)SceneData.sceneData.spell_dmg; // Ouch!
                //GetComponent<Health>().DecreaseHealthGradually(Time.deltaTime, (int)SceneData.sceneData.spell_dmg); // Old health system code for reference in future?
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


    }

    // Destroy all components and gameobject itself
    void OnDestroy()
    {
        foreach (var comp in GetComponents<Component>())
        {
            Destroy(comp);
        }

        
        Destroy(friendlyHealth);
        Destroy(friendlyHealth.transform.GetChild(0).gameObject);
        Destroy(enemyHealth);
        Destroy(enemyHealth.transform.GetChild(0).gameObject);
        if (SpatialPartition.instance)
        {
            SpatialPartition.instance.RemoveGameObject(gameObject); // remove from spatial partitioning
        }
    }

    void DoAttack()
    {
        m_timer.Update();//update attack speed
        if (GetHealth() <=0 )//enemy hp check, enemy die, time to get a new target
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
        else if (ismelee) // A pawn on a chessboard, so can only hit around one grid distance ahead
        {
            float mrange = 60;
            if (ismelee && displacement.sqrMagnitude > mrange) // distannce check
            {
                gameObject.GetComponent<VMovement>().Velocity = displacement.normalized * 35; // move towards enemy if not within attack range
                GetComponent<VMovement>().m_stopMove = false; // Stop moving to fight
                return;
            }
        }

        if (!m_timer.can_run) // atkspeed limit
            return;
        if (!ismelee)
            GetComponent<VMovement>().m_stopMove = true; //stop moving to whack


        // Attack success past this line
        Animator anim = gameObject.transform.GetChild(0).GetComponent<Animator>();
        anim.SetBool("b_attack", true);
        m_timer.Reset(); // reset atk speed timer
        bool cannotPlay = m_audioList[(int)m_type];
        // Switch case for units to play different sounds and do different actions depending on melee or ranged units
        switch (m_type)
        {
            case (UNIT_TYPE.CW_KNIGHT):
                {
                    //probably change later
                    GetComponent<VMovement>().m_stopMove = true;
                    //do damage
                    DoDamage(m_attkDamage);
                    if (cannotPlay)
                    {
                        PlayAudio.instance.m_source.clip = PlayAudio.instance.m_clockworkKnight; // Assign sound clip to singleton PlayAudio class object
                        PlayAudio.instance.m_source.volume = 0.5f; // half-volume
                        PlayAudio.instance.PlayOnce();
                    }
                    m_audioList[(int)UNIT_TYPE.CW_KNIGHT] = true;
                } break;

            case (UNIT_TYPE.BALLISTA):
                {
                    //Instantiate()
                    GameObject bullet = Instantiate(projectile, gameObject.transform.position, Quaternion.identity) as GameObject;
                    bullet.GetComponent<Bprojectile>().setprojectile(m_targetEnemy, m_attkDamage, 1);
                    if (cannotPlay)
                    {
                        PlayAudio.instance.m_source.clip = PlayAudio.instance.m_ballista;
                        PlayAudio.instance.m_source.volume = 0.5f;
                        PlayAudio.instance.PlayOnce();
                    }
                    m_audioList[(int)UNIT_TYPE.BALLISTA] = true;
                } break;

            case (UNIT_TYPE.SPIDER_TNK):
                {
                    //GameObject flash = Instantiate(projectile, gameObject.transform.GetChild(1).transform.position, Quaternion.identity) as GameObject;
                    Emitter.SetActive(true);
                    Emitter.transform.position = gameObject.transform.GetChild(1).transform.position;
                    DoDamage(m_attkDamage);
                    if (cannotPlay)
                    {
                        PlayAudio.instance.m_source.clip = PlayAudio.instance.m_spidertank;
                        PlayAudio.instance.m_source.volume = 0.4f;
                        PlayAudio.instance.PlayOnce();
                    }
                    m_audioList[(int)UNIT_TYPE.SPIDER_TNK] = true;
                } break;
            case (UNIT_TYPE.BBUSTER):
                {
                    Emitter.SetActive(true);
                    Emitter.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y+10, gameObject.transform.position.z);
                    Emitter.transform.rotation = transform.rotation;
                    //Emitter.transform.position.y += 20;
                    DoDamage(m_attkDamage);
                    if (cannotPlay)
                    {
                        PlayAudio.instance.m_source.clip = PlayAudio.instance.m_buster;
                        PlayAudio.instance.m_source.volume = 0.6f;
                        PlayAudio.instance.PlayOnce();
                    }
                    m_audioList[(int)UNIT_TYPE.BBUSTER] = true;
                } break;
            case (UNIT_TYPE.RAILGUN):
                {
                    Emitter.SetActive(true);
                    Emitter.transform.position = gameObject.transform.GetChild(1).transform.position;
                    Vector3 pos = new Vector3(m_targetEnemy.transform.position.x, m_targetEnemy.transform.position.y + 1000, m_targetEnemy.transform.position.z);

                    GameObject blast = Instantiate(projectile, pos, Quaternion.identity) as GameObject;
                    blast.GetComponent<Bprojectile>().setprojectile(m_targetEnemy, m_attkDamage, 2, 400);
                    if (cannotPlay)
                    {
                        //PlayAudio.instance.m_source.clip = PlayAudio.instance.m_railgun;
                        //PlayAudio.instance.m_source.volume = 0.6f;
                        //PlayAudio.instance.PlayOnce();
                    }
                    m_audioList[(int)UNIT_TYPE.RAILGUN] = true;
                } break;
            case (UNIT_TYPE.IEN_GOLEM):
                {
                    //probably change later
                    GetComponent<VMovement>().m_stopMove = true;                    
                    //do damage
                    DoDamage(m_attkDamage);
                    if (cannotPlay)
                    {
                        PlayAudio.instance.m_source.clip = PlayAudio.instance.m_ironGolem;
                        PlayAudio.instance.m_source.volume = 0.6f;
                        PlayAudio.instance.PlayOnce();
                    }
                    m_audioList[(int)UNIT_TYPE.IEN_GOLEM] = true;
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
        if (m_health <= 0) // Unit died
        {
            if (Emitter != null)
                Destroy(Emitter);
            RemoveEntity(this.gameObject);
            Spawn.m_entityList.Remove(gameObject);
            UnityEngine.Object.Destroy(this.gameObject);
            /*Gold*/
            if (!m_isFriendly)
            {
                if(m_player)
                m_player.AddPlayerGold(m_gold);
            }
            m_audioList[(int)m_type] = false;
        }
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






