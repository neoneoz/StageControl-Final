using UnityEngine;
using System.Collections;

public class Opponent : MonoBehaviour {

	// Use this for initialization
    public GameObject Clockwork, Ballista, Bbuster, Spider, Railgun, Irongolem;
    public Building e_base;
    float lastupdatetime ;
    Behaviour currentbehaviour;
    enum BEHAVIOUR
    {
        E_DEFEND,
        E_SEARCH,
        E_EXPAND,
        E_NULL
    }

	void Start () {

        lastupdatetime = 0;
        //currentbehaviour = 
	}
	
	// Update is called once per frame
    void Update()
    {
        if (SceneData.sceneData.Gametime - lastupdatetime < 5f)
            return;
        else
            lastupdatetime = SceneData.sceneData.Gametime;
   
    }


    void peroidicUpdate()
    {

    }

    bool ConstructBuilding(GameObject building , Vector3 pos)
    {
        Instantiate(building);
        pos = SceneData.sceneData.gridmesh.SnapBuildingPos(pos, building.GetComponent<Building>().size);//snap the building to the grid
        if(SceneData.sceneData.gridmesh.DerenderBuildGrids(true))
        {
            return true;
        }
        else
            return false;
    }


}
