﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class SceneData : MonoBehaviour
{
    public static SceneData sceneData;
    public GameObject EntityList;
    public Canvas UI;
    public GridArray gridmesh;
    public Text debuginfo;
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

    /*Spell*/
    // Two values for storing where player is placing spell card
    [HideInInspector]
    public Vector4 spell_grid;
    [HideInInspector]
    public bool is_spellCast; // is the spell cast?
    [HideInInspector]
    public bool is_spellHit; // is the spell cast?
    [HideInInspector]
    public float spell_dmg; // spell damage
    /**/

    //Particles
    public ParticleSystem buildingP;

    public Image buildTimer;
    public Image spawnTimer;

    //Building Health
    public Image Health_friendly;
    public Image Health_enemy;

    //Object ID
    uint ObjectID = 0;

    public uint GetUniqueID()
    {
        ObjectID++;
        return ObjectID - 1;
    }

    void SnapBasesToGrid()
    {
        if (LevelManager.instance)
        {
            LevelManager.instance.PlayerBase.transform.position = SceneData.sceneData.gridmesh.SnapBuildingPos(LevelManager.instance.PlayerBase.transform.position, 4);
            SceneData.sceneData.gridmesh.DerenderBuildGrids(true);
            LevelManager.instance.EnemyBase.transform.position = SceneData.sceneData.gridmesh.SnapBuildingPos(LevelManager.instance.EnemyBase.transform.position, 4);
            SceneData.sceneData.gridmesh.DerenderBuildGrids(true);      
        }
    }

    void Awake()
    {
        sceneData = this;
        Invoke("SnapBasesToGrid", 1);
    }

    void Start()
    {

    }
}
