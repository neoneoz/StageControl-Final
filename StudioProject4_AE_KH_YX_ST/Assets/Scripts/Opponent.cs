using UnityEngine;
using System.Collections;

public class Opponent : MonoBehaviour {

	// Use this for initialization
    public GameObject Clockwork, Ballista, Bbuster, Spider, Railgun, Irongolem;
	void Start () {
	
	}
	
	// Update is called once per frame
    void Update()
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
