using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
    public Image buildTimerTemp = null;
    public Image spawnTimerTemp = null;
    public Image buildingHealthImage = null;
    public float buildTimer;
    private static GameObject m_buildingControl;
    private static bool m_initController = true;
    public static List<GameObject> m_buildingList; // List of all the buildings in the scene
    public float buildingHealth;
    public float maxBuildingHealth;
    public bool isbase = false;
    public bool isVisible = true;

    bool buildgrids = false;

    //ID
    uint ID;

    public uint GetID()
    {
        return ID;
    }

    // Use this for initialization
    void Start()
    {

        //b_state = BUILDSTATE.B_HOLOGRAM;

#if UNITY_ANDROID
        size = size >> 1;
        if (size <= 0)
            size = 1;
#endif

        Invoke("InstantiateParticles", 0.1f);
        //isfriendly = true;//default to the player's units
        m_isDistract = false;
        // The controller all building belongs to
        if (m_initController)
        {
            m_buildingControl = new GameObject();
            m_buildingControl.name = "Building Controller";

            m_buildingList = new List<GameObject>();
            m_initController = false;
        }
        
        transform.SetParent(m_buildingControl.transform);
     

       
        //GameObject handle, handleChild;
        //handle = new GameObject();
        //handleChild = new GameObject();
        //Image img, imgChild;
        ////handle = handleChild = (GameObject)Instantiate(m_entity);
        //handle.AddComponent<Image>();
        //img = handle.GetComponent<Image>();
        //img.transform.SetParent(transform.parent.GetChild(0));
        //img.rectTransform.sizeDelta = new Vector2(50, 10);
        //img.rectTransform.pivot = new Vector2(0f, 0.5f);
        //img.color = Color.red;
        //handleChild.AddComponent<Image>();
        //imgChild = handleChild.GetComponent<Image>();
        //imgChild.transform.SetParent(img.transform);
        //imgChild.rectTransform.sizeDelta = new Vector2(50, 10);
        //imgChild.rectTransform.pivot = new Vector2(0f, 0.5f);
        //imgChild.color = Color.green;
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

        //buildingHealth_friendlyTemp = Instantiate(SceneData.sceneData.Health_friendly);
        //buildingHealth_friendlyTemp.transform.SetParent(SceneData.sceneData.UI.transform);
        //buildingHealth_friendlyTemp.enabled = false;
        //buildingHealth_friendlyTemp.transform.GetChild(0).GetComponent<Image>().enabled = false;

        //buildingHealth_enemyTemp = Instantiate(SceneData.sceneData.Health_enemy);
        //buildingHealth_enemyTemp.transform.SetParent(SceneData.sceneData.UI.transform);
        //buildingHealth_enemyTemp.enabled = false;
        //buildingHealth_enemyTemp.transform.GetChild(0).GetComponent<Image>().enabled = false;
        CreateHealthBar();
        ID = SceneData.sceneData.GetUniqueID();
        SpatialPartition.instance.AddGameObject(gameObject);
    }

    public void CreateHealthBar()
    {
        if (isfriendly)
        {
            buildingHealthImage = Instantiate(SceneData.sceneData.Health_friendly);
            buildingHealthImage.transform.SetParent(SceneData.sceneData.UI.transform);
        }
        else
        {
            buildingHealthImage = Instantiate(SceneData.sceneData.Health_enemy);
            buildingHealthImage.transform.SetParent(SceneData.sceneData.UI.transform);
        }

        if (!isbase)
        {
            buildingHealthImage.enabled = false;
            buildingHealthImage.transform.GetChild(0).GetComponent<Image>().enabled = false;
        }
    }

    public Vector3 GetMaxPosOfBuilding(Vector3 position, int othersize)
    {
        Vector3 maxpos = position + new Vector3(SceneData.sceneData.gridmesh.GridSizeX * (othersize) + SceneData.sceneData.gridmesh.GridSizeX, 0, SceneData.sceneData.gridmesh.GridSizeZ * (othersize) + SceneData.sceneData.gridmesh.GridSizeZ);
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
                        buildingHealthImage.enabled = true;
                        buildingHealthImage.transform.GetChild(0).GetComponent<Image>().enabled = true;
                        GetComponent<Spawn>().SetSpawnPosition();
                        Destroy(buildingTemp);
                        Destroy(buildTimerTemp);
                    }
                }
                break;
            case BUILDSTATE.B_ACTIVE:

                if (PlayAudio.instance.m_soundOwner != null)
                    if (PlayAudio.instance.m_source.isPlaying && PlayAudio.instance.m_soundOwner.Equals(gameObject) && PlayAudio.instance.m_source.time > buildTimer)
                        PlayAudio.instance.m_source.Stop();

                if (buildingHealthImage != null)
                {
                    buildingHealthImage.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position + gameObject.transform.up.normalized * 10);
                    buildingHealthImage.transform.GetChild(0).GetComponent<Image>().fillAmount = buildingHealth / maxBuildingHealth;
                    buildingHealthImage.transform.GetChild(0).GetComponent<Image>().transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position + gameObject.transform.up.normalized * 10);
         
                }

                if (GetComponent<Spawn>() && spawnTimerTemp)
                {
                    spawnTimerTemp.enabled = true;
                    spawnTimerTemp.fillAmount = GetComponent<Spawn>().GetRatio();
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
                        GetComponent<Pathfinder>().FindPath(GetComponent<Spawn>().UnitSpawnPosition, GetMaxPosOfBuilding(LevelManager.instance.EnemyBase.transform.position, LevelManager.instance.EnemyBase.GetComponent<Building>().size));
                    }
                    else if (!isfriendly && GetComponent<Pathfinder>() && LevelManager.instance.PlayerBase)
                    {
                        GetComponent<Pathfinder>().FindPath(GetComponent<Spawn>().UnitSpawnPosition, GetMaxPosOfBuilding(LevelManager.instance.PlayerBase.transform.position, LevelManager.instance.PlayerBase.GetComponent<Building>().size));
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
        PlayAudio.instance.m_source.clip = PlayAudio.instance.m_construct;
        PlayAudio.instance.m_source.Play();
        PlayAudio.instance.m_source.volume = 0.1f;
        PlayAudio.instance.m_source.priority = 1;
        PlayAudio.instance.m_soundOwner = gameObject;
    }

    public void TakeDamage(float damage)
    {
        buildingHealth -= damage;
        if (buildingHealth <= 0)
        {
            //explosionTemp.Play();
            //explosionTemp.transform.position = gameObject.transform.position;
            ExplosionManager.instance.ApplyExplosion(transform.position);
            Destroy(spawnTimerTemp);
            Destroy(buildingHealthImage);
            Destroy(buildingHealthImage.transform.GetChild(0).gameObject);
            SceneData.sceneData.gridmesh.FreeGrids(gameObject);
            Building.m_buildingList.Remove(gameObject);
            Destroy(gameObject);

          if (gameObject == LevelManager.instance.PlayerBase)
              SceneController.GoToScene("DefeatScene");

          else if (gameObject == LevelManager.instance.EnemyBase)
              SceneController.GoToScene("VictoryScene");


        }
        
    }

    void OnDestroy()
    {
        SpatialPartition.instance.RemoveGameObject(gameObject);    
    }
}
