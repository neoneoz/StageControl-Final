using UnityEngine;
using System.Collections;

public class Opponent : MonoBehaviour {

	// Use this for initialization
    public GameObject Clockwork, Ballista, Bbuster, Spider, Railgun, Irongolem;
    public Building e_base;


    int space,dmg,control,coefficient;//descisionmaking vairables
    float lasthp = 3000;//save the hp of the base
    float lastupdatetime ;//time since last update
    BEHAVIOUR currentbehaviour;
    enum BEHAVIOUR
    {
        E_DEFEND,
        E_EXPAND,
        E_NULL
    };

	void Start () {

        lastupdatetime = 0;
        //currentbehaviour = 
	}
	
	// Update is called once per frame
    void Update()
    {

        //run the update in intervals 
        if (SceneData.sceneData.Gametime - lastupdatetime < 5f)
            return;
        else//one-off update stuff in here
        {
           
            lastupdatetime = SceneData.sceneData.Gametime;
            coefficient = GetCoefficient(); 
            lasthp = e_base.GetComponent<Building>().buildingHealth;
            peroidicUpdate();
        }
   
    }


    void peroidicUpdate()
    {
        switchstate(); Vector2 source = SceneData.sceneData.gridmesh.GetGridIndexAtPosition(e_base.transform.position);
        switch(currentbehaviour )
        {
                
            case BEHAVIOUR.E_EXPAND://build more shit futher up
                Debug.Log("expanding");
 
            


               
                break;


            case BEHAVIOUR.E_DEFEND://build more shit futher up
                Debug.Log("Defending");
                //ConstructBuilding(Clockwork, source);
    

                break;


            case BEHAVIOUR.E_NULL://build more shit futher up
                Debug.Log("Nothing");


                break;
        }
    }

    int GetCoefficient()//change tree  probability
    {
       float basehpdiff = (lasthp - e_base.GetComponent<Building>().buildingHealth);//change 300 no not hardcode later

       if(basehpdiff > 50)
       {
           for (int i = (int)basehpdiff; i >= 79; i -= 80)
           {
               dmg+=5;
           }
       }
       else
       {
           space += 10;

       }

       
        //floatspace = (int)e_base.GetComponent<Building>().maxBuildingHealth;
        return space - dmg;
    }

    void switchstate()
    {
        if (space > 50)
        {
            currentbehaviour = BEHAVIOUR.E_EXPAND;
            resetvar();

        }
        if(dmg > 50)
        {
            currentbehaviour = BEHAVIOUR.E_DEFEND;
            resetvar();
        }
        else
        {

            currentbehaviour = BEHAVIOUR.E_NULL;
        }

    }

    void resetvar()
    {
        space = 0;
        dmg = 0;
    }


    //bool ConstructBuilding(GameObject building , Vector2 grid)
    //{
    //    //Vector2 offset = new Vector2(-4,-4);
    //    //Vector3 pos = SceneData.sceneData.gridmesh.SnapBuildingPos(SceneData.sceneData.gridmesh.get, building.GetComponent<Building>().size);//snap the building to the grid
    //    //if (SceneData.sceneData.gridmesh.DerenderBuildGrids(true))
    //    //{
    //    //    Instantiate(building);
    //    //    return true;
    //    //}
    //    //else
    //    //{
            
    //    //    ConstructBuilding(building, pos+offset);
    //    //    return false;
    //    //}
    //}


}
