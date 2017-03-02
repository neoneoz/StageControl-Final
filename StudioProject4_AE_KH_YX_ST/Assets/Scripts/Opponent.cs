using UnityEngine;
using System.Collections;

public class Opponent : MonoBehaviour {

	// Use this for initialization
    public GameObject Clockwork, Ballista, Bbuster, Spider, Railgun, Irongolem;
    public Building e_base;
    public Vector2 source,msource;

    int e_buildcount = 0;
    public int space,dmg,control,coefficient;//descisionmaking vairables
    float lasthp = 3000;//save the hp of the base
    float lastupdatetime ;//time since last update
    public BEHAVIOUR currentbehaviour;
    public enum BEHAVIOUR
    {
        E_DEFEND,
        E_EXPAND,
        E_NULL
    };

	void Start () {

        lastupdatetime = 0;
        //source = SceneData.sceneData.gridmesh.GetGridIndexAtPosition(new Vector3(820,0,615));//order of execution
        source = SceneData.sceneData.gridmesh.GetGridIndexAtPosition(e_base.transform.position);
        msource = source;
        //Debug.Log("source :" +source);
        //source.x += 8;
        //source.y += 8;
        //currentbehaviour = 
	}
	
	// Update is called once per frame
    void Update()
    {

        //run the update in intervals 
        if (lastupdatetime - SceneData.sceneData.Gametime > -5f)
            return;
        else//one-off update stuff in here
        {
            Debug.Log("enemy updating");
            lastupdatetime = SceneData.sceneData.Gametime;
            coefficient = GetCoefficient(); 
            lasthp = e_base.GetComponent<Building>().buildingHealth;
            switchstate();
            peroidicUpdate();
            if(currentbehaviour != BEHAVIOUR.E_NULL)
            {
                resetvar();
            }


            //ConstructBuilding(Clockwork);
        }
   
    }


    void peroidicUpdate()
    {
        if(e_buildcount > 6)
        {
            return;
        }
        switch(currentbehaviour )
        {
             
            case BEHAVIOUR.E_EXPAND://build more shit futher up
                Debug.Log("expanding");
                if (!roll(80))
                {
                    //Debug.Log("failed 80 roll");
                    break;

                }
                if (roll(50))
                {
                    //Debug.Log("balista roll");
                    ConstructBuilding(Ballista);
                    break;
                    
                }
                if (roll(30))
                {
                    //Debug.Log("irongolem roll roll");
                    ConstructBuilding(Irongolem);
                    break;
                }
                if (roll(80))
                {
                    ConstructBuilding(Bbuster);
                    break;
                }


                break;


            case BEHAVIOUR.E_DEFEND://build more near the base
                Debug.Log("Defending");
                source = msource;
                if (roll(60))
                {
                    ConstructBuilding(Railgun);
                    break;
                }
                else
                {
                    ConstructBuilding(Clockwork);
                }
                break;
               


            case BEHAVIOUR.E_NULL:
                Debug.Log("Nothing");
                if (roll(5))
                {
                    ConstructBuilding(Spider);
                    break;
                }
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
               dmg+=20;
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
        if (space > 30)
        {
            currentbehaviour = BEHAVIOUR.E_EXPAND;
            return;
  

        }
        if(dmg > 50)
        {
            currentbehaviour = BEHAVIOUR.E_DEFEND;
            return;
  

        }
        else
        {

            currentbehaviour = BEHAVIOUR.E_NULL;
            return;
  

        }

    }

    void resetvar()
    {
        space = 0;
        dmg = 0;
    }

    bool roll(float chance)//chance in %
    {
        chance = chance*0.01f;
        if (Random.value <= chance)
            return true;
        else
            return false;
    }

    void ConstructBuilding(GameObject building)
    {
        Vector2 offset;
        offset = new Vector2(0,0);
        switch (currentbehaviour)
        {

            case BEHAVIOUR.E_EXPAND://build more shit futher up
            for (int i = 0; i < 4; i++)
            { 
            offset.x= Random.Range(-8,2);
            offset.y = Random.Range(-8,2);
            offset = source + offset;
            
            
            if (trybuild(building, offset))
                break;

            }
        


                break;


            case BEHAVIOUR.E_DEFEND://build more shit futher up
            for (int i = 0; i < 4; i++)
            {
            offset.x= Random.Range(-6,4);
            offset.y = Random.Range(-6,4);
            offset = source + offset;
            if (trybuild(building, offset))
                break;

            }
   
                break;


            case BEHAVIOUR.E_NULL://build more shit futher up
            for (int i = 0; i < 4; i++)
            {
            offset.x= Random.Range(-8,8);
            offset.y = Random.Range(-8,8);
            offset= source + offset;

            if (trybuild(building, offset))
                break;

            }


                break;
        }

        //get varied position 
       
        
        //Debug.Log("source :" + source);

    }

    bool trybuild(GameObject building,Vector2 grid)
    {
        if (grid.x >SceneData.sceneData.gridmesh.m_rows  || grid.x <= 0)
            return false;

        if (grid.y >SceneData.sceneData.gridmesh.m_columns || grid.y <= 0)
            return false;



        Vector3 pos = SceneData.sceneData.gridmesh.GetPositionAtGrid((int)grid.x, (int)grid.y);//get the position frm grid
        pos = SceneData.sceneData.gridmesh.SnapBuildingPos(pos, building.GetComponent<Building>().size,false);//snap the building to the grid
        if (SceneData.sceneData.gridmesh.ForceConstruct(building, pos))
        {
            Debug.Log("Ai build");
            GameObject newbuild = Instantiate(building);
            newbuild.SetActive(true);
            newbuild.transform.position = pos;
            newbuild.GetComponent<Building>().isfriendly = false;
            newbuild.GetComponent<Building>().b_state = Building.BUILDSTATE.B_CONSTRUCT;
            changebuildcount(true);
            source = grid;
            return true;
        }
        else
        {
            Debug.Log("Ai build failed");
            return false;
        }

    }
    
    void changebuildcount(bool isadd)
    {
        if (isadd)//built something
            e_buildcount++;
        else
            e_buildcount--;
    }

}
