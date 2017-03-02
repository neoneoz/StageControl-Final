﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class SceneData : MonoBehaviour
{
    public static SceneData sceneData;
    public GameObject EntityList;
    public Canvas UI;
    public GridArray gridmesh;
    public Text debuginfo,GameTimer;
    public Terrain ground;
    public PlayerInfo Player;
    public Camera camera;

    //Decks
    public GameObject PlayerDeck = null;
    public bool isHoldingCard = false;

    //handlers
    public HandHandler handhandler;
    public DragHandler draghandler;

    //Flocking Settings
    public float CohesionWeight = 1f;
    public float SeperationWeight = 1f;
    public float AlignmentWeight = 1f;
    public float Gametime = 0;

    /*Spell*/
    // Two values for storing where player is placing spell card
    [HideInInspector]
    public Vector4 spell_grid;
    public float spell_dmg; // spell damage
    /**/

    //Particles
    public ParticleSystem buildingP;
    public ParticleSystem explosionP;

    public Image buildTimer;
    public Image spawnTimer;

    //Building Health
    public Image Health_friendly;
    public Image Health_enemy;

    //Spawn limits
#if UNITY_ANDROID
    public float SPAWN_LIMIT = 8;
#else
    public float SPAWN_LIMIT = 30;
#endif
    public float p_unitcount, e_unitcount;
    public bool p_unitmax, e_unitmax;



    //Object ID
    uint ObjectID = 0;

    //New Deck Card  Game Object
    public Button NewDeckButton;
    public Image fireSPrite;
                                                                                                                                                        
    public uint GetUniqueID()
    {
        ObjectID++;
        return ObjectID - 1;
    }
    void Update()
    {
        Gametime += Time.deltaTime;
        //GameTimer.text = Gametime.ToString();
    }
    void SnapBasesToGrid()
    {

        if (LevelManager.instance)
        {


            SceneData.sceneData.gridmesh.Reset();

#if UNITY_ANDROID
            LevelManager.instance.PlayerBase.transform.position = SceneData.sceneData.gridmesh.SnapBuildingPos(LevelManager.instance.PlayerBase.transform.position, 2);
#else
            LevelManager.instance.PlayerBase.transform.position = SceneData.sceneData.gridmesh.SnapBuildingPos(LevelManager.instance.PlayerBase.transform.position, 4);
#endif
            SceneData.sceneData.gridmesh.DerenderBuildGrids(true,true);

            SceneData.sceneData.gridmesh.Reset();
#if UNITY_ANDROID
            LevelManager.instance.EnemyBase.transform.position = SceneData.sceneData.gridmesh.SnapBuildingPos(LevelManager.instance.EnemyBase.transform.position, 2);
#else
            LevelManager.instance.EnemyBase.transform.position = SceneData.sceneData.gridmesh.SnapBuildingPos(LevelManager.instance.EnemyBase.transform.position, 4);
#endif
            SceneData.sceneData.gridmesh.DerenderBuildGrids(true,true);

            SceneData.sceneData.gridmesh.SetBuildableGrids(LevelManager.instance.PlayerBase);
              
        }
    }
    public void AddUnitCount(bool isfriendly)
    { 
        if(isfriendly)
            ++p_unitcount;
        else
            ++e_unitcount;
        updateUnitcount();
    }

    public void RemoveUnitCount(bool isfriendly)
    {
        if (isfriendly)
           ++p_unitcount;
        else
           ++e_unitcount;
        updateUnitcount();
    }

    void updateUnitcount()
    {
        if (p_unitcount >= SPAWN_LIMIT)
        {
            p_unitmax = true;
        }
        else
            p_unitmax = false;


        if (e_unitcount >= SPAWN_LIMIT)
        {
            e_unitmax = true;
        }
        else
            e_unitmax = false;

    }
    void Awake()
    {
        sceneData = this;
        Invoke("SnapBasesToGrid",1);
    }

    void Start()
    {
        p_unitcount = 0;
        e_unitcount = 0;
        p_unitmax=  false;
        e_unitmax = false;
        //SnapBasesToGrid();
    }
}
