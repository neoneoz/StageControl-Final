using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class Building : MonoBehaviour
{
    //base class for all buildings
    //call the spawning stuff here samuel, mai la, mai la, wa mai la
    public enum BUILDSTATE
    {
        B_HOLOGRAM,
        B_CONSTRUCT,
        B_ACTIVE
    };
    //public GameObject Unit; //the unit that this building spawns, spawn script already requires a unit
    public float buildtime, spawntime;// time to construct the building/time it takes to spawn a single unit
    // Max number of troops the building can spawn
    public static int MAX_UNIT = 30;
    public int size;//building size
    public Material holo, undamaged, damaged;
    public BUILDSTATE b_state;
    public bool isfriendly;
    public bool m_isDistract;
    float timerB = 0.0f;
    ParticleSystem buildingTemp = null;
    Image buildTimerTemp = null;
    Image spawnTimerTemp = null;
    Image buildingHealth_friendlyTemp = null;
    Image buildingHealth_enemyTemp = null;
    public float buildTimer;
    private static GameObject m_buildingControl;
    private static bool m_initController = true;
    public static List<GameObject> m_buildingList; // List of all the buildings in the scene
    public float buildingHealth;
    public float maxBuildingHealth;

    // Use this for initialization
    void Start()
    {
        //b_state = BUILDSTATE.B_HOLOGRAM;

        Invoke("InstantiateParticles", 0.1f);
        //isfriendly = true;//default to the player's units
        m_isDistract = false;
        // The controller all building belongs to
        if (m_initController)
        {
            m_buildingControl = new GameObject();
            m_buildingControl.name = "Building Controller";
            //GameObject temporary = new GameObject();
            //Canvas temporary_canvas = temporary.AddComponent<Canvas>();
            //temporary_canvas.transform.SetParent(m_buildingControl.transform);
            //temporary_canvas.renderMode = RenderMode.WorldSpace;
            m_buildingList = new List<GameObject>();
            m_initController = false;
        }
        transform.SetParent(m_buildingControl.transform);
        GameObject handle, handleChild;
        handle = new GameObject();
        handleChild = new GameObject();
        Image img, imgChild;
        //handle = handleChild = (GameObject)Instantiate(m_entity);
        handle.AddComponent<Image>();
        img = handle.GetComponent<Image>();
        img.transform.SetParent(transform.parent.GetChild(0));
        img.rectTransform.sizeDelta = new Vector2(50, 10);
        img.rectTransform.pivot = new Vector2(0f, 0.5f);
        img.color = Color.red;
        handleChild.AddComponent<Image>();
        imgChild = handleChild.GetComponent<Image>();
        imgChild.transform.SetParent(img.transform);
        imgChild.rectTransform.sizeDelta = new Vector2(50, 10);
        imgChild.rectTransform.pivot = new Vector2(0f, 0.5f);
        imgChild.color = Color.green;
        m_buildingList.Add(gameObject);
    }

    void InstantiateParticles()
    {
        buildingTemp = Instantiate(SceneData.sceneData.buildingP);
        buildTimerTemp = Instantiate(SceneData.sceneData.buildTimer);
        buildTimerTemp.transform.SetParent(SceneData.sceneData.UI.transform);
        buildTimerTemp.enabled = false;

        spawnTimerTemp = Instantiate(SceneData.sceneData.spawnTimer);
        spawnTimerTemp.transform.SetParent(SceneData.sceneData.UI.transform);
        spawnTimerTemp.enabled = false;

        buildingHealth_friendlyTemp = Instantiate(SceneData.sceneData.Health_friendly);
        buildingHealth_friendlyTemp.transform.SetParent(SceneData.sceneData.UI.transform);
        buildingHealth_friendlyTemp.enabled = false;
        buildingHealth_friendlyTemp.transform.GetChild(0).GetComponent<Image>().enabled = false;

        buildingHealth_enemyTemp = Instantiate(SceneData.sceneData.Health_enemy);
        buildingHealth_enemyTemp.transform.SetParent(SceneData.sceneData.UI.transform);
        buildingHealth_enemyTemp.enabled = false;
        buildingHealth_enemyTemp.transform.GetChild(0).GetComponent<Image>().enabled = false;
    }
    

    public Vector3 GetMaxPosOfBuilding(Vector3 position, int othersize)
    {
        Vector3 maxpos = position + new Vector3(SceneData.sceneData.gridmesh.GridSizeX * (othersize), 0, SceneData.sceneData.gridmesh.GridSizeX * (othersize));
        return maxpos;
    }

    // Update is called once per frame
    void Update()
    {

        switch (b_state)
        {
            case BUILDSTATE.B_HOLOGRAM:
                for (int i = 0; i < gameObject.transform.GetChild(0).childCount; ++i)
                {
                    gameObject.transform.GetChild(0).transform.GetChild(i).GetComponent<MeshRenderer>().material = holo;

                }

                break;
            case BUILDSTATE.B_CONSTRUCT:
                timerB += Time.deltaTime;
                if (timerB < buildTimer)
                {
                    if (buildingTemp)
                    {
                        buildingTemp.Play();
                        buildingTemp.transform.position = gameObject.transform.position;
                        buildTimerTemp.fillAmount += 1.0f / buildTimer * Time.deltaTime;
                        buildTimerTemp.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position);
                        for (int i = 0; i < gameObject.transform.GetChild(0).childCount; ++i)
                        {
                            gameObject.transform.GetChild(0).transform.GetChild(i).GetComponent<MeshRenderer>().material = holo;
                        }
                    }
                }
                else if (timerB >= buildTimer)
                {
                    if (buildingTemp)
                    {
                        buildingTemp.Stop();
                        buildingTemp.transform.position = gameObject.transform.position;
                        for (int i = 0; i < gameObject.transform.GetChild(0).childCount; ++i)
                        {
                            gameObject.transform.GetChild(0).transform.GetChild(i).GetComponent<MeshRenderer>().material = undamaged;
                        }
                        b_state = BUILDSTATE.B_ACTIVE;
                        Destroy(buildingTemp);
                        Destroy(buildTimerTemp);
                    }
                }
                break;
            case BUILDSTATE.B_ACTIVE:
                if (isfriendly == true && buildingHealth_friendlyTemp != null)
                {
                    buildingHealth_friendlyTemp.enabled = true;
                    buildingHealth_friendlyTemp.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position + gameObject.transform.up.normalized * 10);

                    buildingHealth_friendlyTemp.transform.GetChild(0).GetComponent<Image>().enabled = true;
                    buildingHealth_friendlyTemp.transform.GetChild(0).GetComponent<Image>().fillAmount = buildingHealth / maxBuildingHealth;
                    buildingHealth_friendlyTemp.transform.GetChild(0).GetComponent<Image>().transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position + gameObject.transform.up.normalized * 10);
         
                }

                else if (isfriendly == false && buildingHealth_enemyTemp != null)
                {
                    buildingHealth_enemyTemp.enabled = true;
                    buildingHealth_enemyTemp.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position + gameObject.transform.up.normalized * 10);

                    buildingHealth_enemyTemp.transform.GetChild(0).GetComponent<Image>().enabled = true;
                    buildingHealth_enemyTemp.transform.GetChild(0).GetComponent<Image>().fillAmount = buildingHealth / maxBuildingHealth;
                    buildingHealth_enemyTemp.transform.GetChild(0).GetComponent<Image>().transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position + gameObject.transform.up.normalized * 10);

                }

                if (GetComponent<Spawn>() && spawnTimerTemp)
                {
                    spawnTimerTemp.enabled = true;
                    spawnTimerTemp.fillAmount = GetComponent<Spawn>().m_timer.GetRatio();
                    spawnTimerTemp.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position);
                }

                
                
                for (int i = 0; i < gameObject.transform.GetChild(0).childCount; ++i)
                {
                    gameObject.transform.GetChild(0).transform.GetChild(i).GetComponent<MeshRenderer>().material = undamaged;
                }

                if (!m_isDistract)
                {
                    if (isfriendly && GetComponent<Pathfinder>() && LevelManager.instance.EnemyBase)
                    {
                        GetComponent<Pathfinder>().FindPath(GetMaxPosOfBuilding(LevelManager.instance.EnemyBase.transform.position, LevelManager.instance.EnemyBase.GetComponent<Building>().size));
                    }
                    else if (!isfriendly && GetComponent<Pathfinder>() && LevelManager.instance.PlayerBase)
                    {
                        GetComponent<Pathfinder>().FindPath(GetMaxPosOfBuilding(LevelManager.instance.PlayerBase.transform.position, LevelManager.instance.PlayerBase.GetComponent<Building>().size));
                    }
                }
                
                //if (GetComponent<Health>().GetHealth() < 0)
                //{
                //    Unit.m_destroyerOfWorlds = new Component[100];
                //    Unit.RemoveEntity(gameObject, m_buildingList);
                //    UnityEngine.Object.Destroy(this.gameObject);
                //    //Unit.m_destroyerOfWorlds = GetComponents(typeof(Component));
                //    //for (int i = 0; i < Unit.m_destroyerOfWorlds.Length; ++i)
                //    //{
                //    //    if (Unit.m_destroyerOfWorlds[i].gameObject.activeSelf)
                //    //        UnityEngine.Object.Destroy(Unit.m_destroyerOfWorlds[i]);
                //    //}
                //}

                break;
           

        }

    }

    public void SetBuilding()
    {
        b_state = BUILDSTATE.B_CONSTRUCT;
        buildTimerTemp.enabled = true;
    }

    public void TakeDamage(float damage)
    {
        buildingHealth -= damage;
        
    }
}
