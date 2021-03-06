﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GridArray : MonoBehaviour
{
    public GameObject StartingGrid;
    public Terrain ground;
    public int GridSizeX = 10;
    public int GridSizeZ = 10;
    public float GridHyp;
    public bool isGenerated = false;
    public int m_rows;
    public int m_columns;
    public GameObject[,] gridmesh;
    public Text debugtext;
    public Vector2 tempmax, tempmin;
    private float SlopeLeniency;
    private float BuildLeniency;
#if UNITY_ANDROID
    public float AndroidSlopeLeniency = 15;
    public float AndroidBuildLeniency = 2;
#elif UNITY_STANDALONE_WIN
    public float PCSlopeLeniency = 10;
    public float PCBuildLeniency = 1;
#endif

    // Use this for initialization
    void Start()
    {
#if UNITY_ANDROID
        SlopeLeniency = AndroidSlopeLeniency;
        BuildLeniency = AndroidBuildLeniency;
#elif UNITY_STANDALONE_WIN
        SlopeLeniency = PCSlopeLeniency;
        BuildLeniency = PCBuildLeniency;
#endif

#if UNITY_ANDROID
        GridSizeX = GridSizeX * 2;
        GridSizeZ = GridSizeZ * 2;
#endif
        GenerateGrid();
        GridHyp = Mathf.Sqrt(GridSizeX * GridSizeX + GridSizeZ * GridSizeZ);

    }
    // Gets gameobject at position passed in or returns null if there is nothing there
    public GameObject GetGridAtPosition(Vector3 position)
    {
        int index_x = (int)(position.x - GridSizeX * 0.5f) / GridSizeX;
        int index_z = (int)(position.z - GridSizeZ * 0.5f) / GridSizeZ;

        if (index_x >= 0 && index_x < m_rows &&
            index_z >= 0 && index_z < m_columns)
        {
            return gridmesh[index_x, index_z];
        }

        return null;
    }

    public Vector3 SnapBuildingPos(Vector3 position , float size,bool render = true)
    {
        float offset = (size - 1f);
        Vector3 maxpos = new Vector3(position.x + (GridSizeX *0.5f) * offset, position.y, position.z + (GridSizeZ * 0.5f) * offset);
        GameObject max = GetGridAtPosition(maxpos);
        Vector3 snaplocation = max.GetComponent<Grid>().GetWorldPosition();
        snaplocation.z -= (GridSizeZ*0.5f) * offset;
        snaplocation.x -= (GridSizeX*0.5f) * offset;
        snaplocation.y = SceneData.sceneData.ground.SampleHeight(snaplocation);

        if (render)
        RenderBuildGrids(max, size);

        //max.GetComponent<Grid>().ChangeState(Grid.GRID_STATE.UNAVAILABLE);   
        return snaplocation;
     
            
    }//takes a position , and a building's size and snaps it to the closet correct position

    public void FreeGrids(GameObject building)//call this to free a building's grids after it is destroyed
    {

        float offset = (building.GetComponent<Building>().size - 1f);
        Vector3 position = building.transform.position;
        Vector3 maxpos = new Vector3(position.x + (GridSizeX * 0.5f) * offset, position.y, position.z + (GridSizeZ * 0.5f) * offset);
        GameObject max = GetGridAtPosition(maxpos);//set the max grid
        
        
        float scale = building.GetComponent<Building>().size - 1;
        //Vector3 maxpos = max.GetComponent<Grid>().GetWorldPosition();
        Vector2 mxIndex = new Vector2(max.GetComponent<Grid>().position.x, max.GetComponent<Grid>().position.y);
        Vector2 mnIndex = new Vector2(mxIndex.x - scale, mxIndex.y - scale);
        int maxX = (int)mxIndex.x; int minX = (int)mnIndex.x;
        int maxY = (int)mxIndex.y; int minY = (int)mnIndex.y;

        for (int i = minX; i <= maxX; ++i)
        {
            for (int j = minY; j <= maxY; ++j)
            {
                if (building.GetComponent<Building>().isfriendly)
                {
                    gridmesh[i, j].GetComponent<Grid>().state = Grid.GRID_STATE.BUILD_AVAILABLE;
                }
                else
                {
                    gridmesh[i, j].GetComponent<Grid>().state = Grid.GRID_STATE.AVAILABLE;
                }
                gridmesh[i, j].GetComponent<Grid>().UpdateAvailability();

            }
        }
    }

    public void RenderRadius(Vector3 mouse_pos, float radius, ref Vector2 oldCoord)
    {
        float offset = (radius - 1f);
        Vector3 maxpos = new Vector3(mouse_pos.x + (GridSizeX * 0.5f) * offset, mouse_pos.y, mouse_pos.z + (GridSizeZ * 0.5f) * offset);
        GameObject max = GetGridAtPosition(maxpos);
        float scale = radius - 1;
        //Vector3 maxpos = max.GetComponent<Grid>().GetWorldPosition();
        Vector2 mxIndex = new Vector2(max.GetComponent<Grid>().position.x, max.GetComponent<Grid>().position.y);
        Vector2 mnIndex = new Vector2(mxIndex.x - scale, mxIndex.y - scale);
        //int diffX = index_maxx - (index_minx + 1);
        //int diffZ = index_maxz - (index_minz + 1);
        //int maxX = (int)mxIndex.x; int minX = (int)mnIndex.x;
        //int maxY = (int)mxIndex.y; int minY = (int)mnIndex.y;

            gridmesh[(int)oldCoord.x, (int)oldCoord.y].GetComponent<Renderer>().enabled = false;
            gridmesh[(int)mnIndex.x, (int)mnIndex.y].GetComponent<Renderer>().enabled = true;
            gridmesh[(int)mnIndex.x, (int)mnIndex.y].GetComponent<Renderer>().material = gridmesh[(int)mnIndex.x, (int)mnIndex.y].GetComponent<Grid>().materials[1];
            oldCoord = mnIndex;
    }

    public void EraseRadius(Vector2 index)
    {
        gridmesh[(int)index.x, (int)index.y].GetComponent<Renderer>().enabled = false;
    }

    public Vector4 GetMouseGrid(Vector3 position, float size)
    {
        float offset = (size - 1f);
        Vector3 maxpos = new Vector3(position.x + (GridSizeX * 0.5f) * offset, position.y, position.z + (GridSizeZ * 0.5f) * offset);
        GameObject max = GetGridAtPosition(maxpos);
        float scale = size - 1;
        Vector2 mxIndex = new Vector2(max.GetComponent<Grid>().position.x, max.GetComponent<Grid>().position.y);
        Vector2 mnIndex = new Vector2(mxIndex.x - scale, mxIndex.y - scale);
        int maxX = (int)mxIndex.x; int minX = (int)mnIndex.x;
        int maxY = (int)mxIndex.y; int minY = (int)mnIndex.y;
        return new Vector4(minX, minY, maxX, maxY);
    }

    public bool CheckWithinRadius(Vector3 ent_pos, Vector4 gridMinMax)
    {
        Vector2 ent_grid = GetGridIndexAtPosition(ent_pos);
        if (ent_grid.x >= gridMinMax.x && ent_grid.x <= gridMinMax.z)
        {
            if (ent_grid.y >= gridMinMax.y && ent_grid.y <= gridMinMax.w)
            {
                return true;
            }
        }
        return false;
    }

    public void Reset()
    {
        tempmax = new Vector2(0,0);
        tempmin = new Vector2(0, 0);
    }

    public void RenderBuildGrids(GameObject max, float size)
    {
        //Debug.Log("showing grid");
        DerenderBuildGrids(false);
        float scale = size -1;
        //Vector3 maxpos = max.GetComponent<Grid>().GetWorldPosition();
        Vector2 mxIndex = new Vector2(max.GetComponent<Grid>().position.x, max.GetComponent<Grid>().position.y);
        Vector2 mnIndex = new Vector2(mxIndex.x - scale, mxIndex.y - scale);
        //int diffX = index_maxx - (index_minx + 1);
        //int diffZ = index_maxz - (index_minz + 1);
        int maxX = (int)mxIndex.x; int minX = (int)mnIndex.x;
        int maxY = (int)mxIndex.y; int minY = (int)mnIndex.y;
        for (int i = minX ; i <= maxX; ++i)
        {
            for (int j = minY ; j <= maxY; ++j)
            {
                gridmesh[i, j].GetComponent<Renderer>().enabled = true;
                if (!gridmesh[i, j].GetComponent<Grid>().buildable)
                {
                    gridmesh[i, j].GetComponent<Renderer>().material = gridmesh[i, j].GetComponent<Grid>().materials[1];
                }
                //else
                //{
                //    gridmesh[i, j].GetComponent<Grid>().UpdateAvailability();
                //}
         
            }
        }
        //store the min max of rendered gfrids
        tempmax = mxIndex;
        tempmin = mnIndex;
    }//renders the grids saved in[tempmn, tempmax]

    public bool DerenderBuildGrids(bool isbuild,bool enemy = false)
    {
        bool buildsucess = true;

        if (isbuild && !enemy)
        {
            for (int i = (int)tempmin.x; i <= (int)tempmax.x; ++i)
            {   for (int j = (int)tempmin.y; j <= (int)tempmax.y; ++j)
                {
                    //Debug.Log("X: " + i + " Y: " + j);
                    if (gridmesh[i, j].GetComponent<Grid>().state != Grid.GRID_STATE.BUILD_AVAILABLE || !gridmesh[i, j].GetComponent<Grid>().buildable)
                    {
                        buildsucess = false;//there is a unavailble slot. Return false and send card back to hand;
                        return buildsucess;
                    }
                }
            }
        }

        for (int i = (int)tempmin.x; i <= (int)tempmax.x; ++i)
        {
            for (int j = (int)tempmin.y; j <= (int)tempmax.y; ++j)
            {

                if(isbuild)//is it render or building(boolean teaken in)
                gridmesh[i, j].GetComponent<Grid>().ChangeState(Grid.GRID_STATE.UNAVAILABLE);

                gridmesh[i, j].GetComponent<Renderer>().enabled = false;
                
            }
        }

        return buildsucess;
    }//derenders the grids saved in [tempmin.tempmax], if isbuild, constructs buildinjg and updates grids




    public void SetBuildableGrids(GameObject basepos)//sets grids aroun the base's area to be buildable
    {
        float size = 28;//square sides in terms of grids

#if UNITY_ANDROID
        size *= 0.5f;
#endif
        Vector2 maxgrid = GetGridIndexAtPosition(basepos.transform.position);//get the grid below the base building
        Vector2 mingrid = maxgrid - new Vector2(size, size);//find minimum grid
        maxgrid += new Vector2(size, size);//find max grid


        for (int i = (int)mingrid.x; i <= (int)maxgrid.x; ++i)
        {
            if (i < 0 || i > m_rows)
                continue;
            for (int j = (int)mingrid.y; j <= (int)maxgrid.y; ++j)
            {
                if (j < 0 || j > m_columns)
                    continue;
                //Debug.Log("X: " + i + " Y: " + j);
                if (gridmesh[i, j].GetComponent<Grid>().state == Grid.GRID_STATE.AVAILABLE)
                {
                    gridmesh[i, j].GetComponent<Grid>().state = Grid.GRID_STATE.BUILD_AVAILABLE;
                    gridmesh[i, j].GetComponent<Grid>().UpdateAvailability();


                }
            }
        }


    }

    public bool ForceConstruct(GameObject building, Vector3 pos)//ai function to help find a place to build a building
    {
        int size = building.GetComponent<Building>().size;
#if UNITY_ANDROID
        size = size >> 1;
        if (size <= 0)
            size = 1;
#endif
        float offset = (size - 1f);
        Vector3 position = pos;
        Vector3 maxpos = new Vector3(position.x + (GridSizeX * 0.5f) * offset, position.y, position.z + (GridSizeZ * 0.5f) * offset);
        GameObject max = GetGridAtPosition(maxpos);
        Vector2 mxIndex = new Vector2(max.GetComponent<Grid>().position.x, max.GetComponent<Grid>().position.y);
        Vector2 mnIndex = new Vector2(mxIndex.x - offset, mxIndex.y - offset);


        int maxX = (int)mxIndex.x; int minX = (int)mnIndex.x;
        int maxY = (int)mxIndex.y; int minY = (int)mnIndex.y;
        for (int i = minX; i <= maxX; ++i)
        {
            for (int j = minY; j <= maxY; ++j)
            {
                if (gridmesh[i, j].GetComponent<Grid>().state == Grid.GRID_STATE.UNAVAILABLE)
                    return false;
                gridmesh[i, j].GetComponent<Grid>().UpdateAvailability();

            }
        }

        for (int i = minX; i <= maxX; ++i)
        {
            for (int j = minY; j <= maxY; ++j)
            {
                gridmesh[i, j].GetComponent<Grid>().state = Grid.GRID_STATE.UNAVAILABLE;
                gridmesh[i, j].GetComponent<Grid>().UpdateAvailability();

            }
        }
        return true;

             

    }

    // Takes in a gameobject position and scale and returns which grids it occupies in the form on their grid index x and y
    public Vector2[] GetOccupiedGrids(Vector3 position, Vector3 scale)
    {
        int index_x = (int)(position.x - GridSizeX * 0.5f) / GridSizeX;
        int index_z = (int)(position.z - GridSizeZ * 0.5f) / GridSizeZ;
        float halfScaleX = (scale.x * 0.5f - GridSizeX * 0.5f) / GridSizeX;
        float halfScaleZ = (scale.z * 0.5f - GridSizeZ * 0.5f) / GridSizeZ;//scale.z / 10f * 0.5f;
        Vector3 minpos = new Vector3(position.x + halfScaleX, position.y, position.z + halfScaleZ);
        Vector3 maxpos = new Vector3(position.x - halfScaleX, position.y, position.z - halfScaleZ);
        //Vector3 min = new Vector3(position.x + 10, position.y, position.z + 10);
        //Vector3 max = new Vector3(position.x - 10, position.y, position.z - 10);

        int index_minx = (int)(minpos.x - GridSizeX * 0.5f) / GridSizeX;
        int index_minz = (int)(minpos.z - GridSizeZ * 0.5f) / GridSizeZ;
        int index_maxx = (int)(maxpos.x - GridSizeX * 0.5f) / GridSizeX;
        int index_maxz = (int)(maxpos.z - GridSizeZ * 0.5f) / GridSizeZ;

        for (int i = index_minx + 1; i >= index_maxx; --i)
        {
            for (int j = index_minz + 1; j >= index_maxz; --j)
            {
                //Debug.Log("X: " + i + " Y: " + j);
                gridmesh[i, j].GetComponent<Grid>().ChangeState(Grid.GRID_STATE.UNAVAILABLE);
                gridmesh[i, j].GetComponent<Renderer>().enabled = true;
            }
        }

        if (index_x >= 0 && index_x <= m_rows &&
            index_z >= 0 && index_z <= m_columns)
        {
            //return gridmesh[index_x, index_z];
        }

        return null;
    }

    // Used to highlight a grid that unit is standing on
    public void HighlightUnitPosition(Vector2 index)
    {
        gridmesh[(int)index.x, (int)index.y].GetComponent<Grid>().ChangeState(Grid.GRID_STATE.INCLOSELIST);
        gridmesh[(int)index.x, (int)index.y].GetComponent<Grid>().EnableRendering(false);
    }

    public Vector3 GetGridPosition(Grid grid)
    {
        if (grid.position.x >= 0 && grid.position.x <= m_rows &&
            grid.position.y >= 0 && grid.position.y <= m_columns)
        {
            return gridmesh[(int)grid.position.x, (int)grid.position.y].GetComponent<Grid>().position;
        }

        return new Vector3(0, 0, 0);
    }

    public Grid GetGridObjAtPosition(Vector3 pos)
    {
        int index_x = (int)(pos.x - GridSizeX * 0.5f) / GridSizeX;
        int index_z = (int)(pos.z - GridSizeZ * 0.5f) / GridSizeZ;

        if (index_x >= 0 && index_x <= m_rows &&
            index_z >= 0 && index_z <= m_columns)
        {
            return gridmesh[index_x, index_z].GetComponent<Grid>();
        }
        return null;
    }

    // Gets grid index in Vec2 at position argument or returns zero vector if theres none
    public Vector3 GetGridIndexAtPosition(Vector3 position)
    {
        // index_x and z are coordinates offsetted by (half of size of 1 grid) and converted to grid coordinates
        int index_x = (int)(position.x - GridSizeX * 0.5f) / GridSizeX;
        int index_z = (int)(position.z - GridSizeZ * 0.5f) / GridSizeZ;

        if (index_x >= 0 && index_x <= m_rows &&
            index_z >= 0 && index_z <= m_columns)
        {
            return new Vector3(index_x, index_z);
        }

        return Vector2.zero;
    }


    // Gets position from supplied grid coordinates
    public Vector3 GetPositionAtGrid(int gridx_, int gridz_)
    {
        float positionX = gridx_ * GridSizeX + GridSizeX * 0.5f; // might lose precision at GetGridAtPosition's static conversion to int but its negligible
        float positionZ = gridz_ * GridSizeZ + GridSizeZ * 0.5f;
        return new Vector3(positionX, 0, positionZ);
    }

    public float GetTerrainHeightAtGrid(Vector3 pos)
    {
        return ground.SampleHeight(pos);
    }

    bool isGridCollidingWithTerrain(Grid grid)
    {
        float highestvalue = 0;
        if (highestvalue < grid.Points[0].y - grid.Points[1].y)
            highestvalue = grid.Points[0].y - grid.Points[1].y;

        if (highestvalue < grid.Points[1].y - grid.Points[2].y)
            highestvalue = grid.Points[1].y - grid.Points[2].y;

        if (highestvalue < grid.Points[2].y - grid.Points[3].y)
            highestvalue = grid.Points[2].y - grid.Points[3].y;

        if (highestvalue < grid.Points[3].y - grid.Points[4].y)
            highestvalue = grid.Points[3].y - grid.Points[4].y;



        if (highestvalue > BuildLeniency)
            grid.buildable = false;
        else
            grid.buildable = true;

        if (highestvalue > SlopeLeniency)
            return true;
        return false;
    }

    void UpdateGridAvailability(Grid grid)
    {
        bool isColliding = isGridCollidingWithTerrain(grid);

        if (isColliding)
        {
            grid.state = Grid.GRID_STATE.UNAVAILABLE;
            grid.UpdateAvailability();
        }
    }

    void GenerateGrid()
    {
        if (isGenerated)
        {
            gridmesh = new GameObject[m_rows, m_columns];
            for (int i = 0; i < transform.childCount; ++i)
            {
                int index_x = (int)transform.GetChild(i).GetComponent<Grid>().position.x;
                int index_z = (int)transform.GetChild(i).GetComponent<Grid>().position.y;
                gridmesh[index_x, index_z] = transform.GetChild(i).gameObject;
            }
                return;
        }


        m_rows = (int)ground.terrainData.size.x / GridSizeX;
        m_columns = (int)ground.terrainData.size.z / GridSizeZ;
        gridmesh = new GameObject[m_rows, m_columns];

        float halfGridSizeX = GridSizeX * 0.5f;
        float halfGridSizeZ = GridSizeZ * 0.5f;
                float highestYvalue = 0f;

        // Create rows
        for (int x = 0; x < m_rows; x++)
        { 
             // Create columns
            for (int z = 0; z < m_columns; z++)
            {
                GameObject grid = (GameObject)Instantiate(StartingGrid);
                grid.name = "Row: " + x + " Col: " + z;
                grid.GetComponent<Grid>().position.x = x;
                grid.GetComponent<Grid>().position.y = z;
                float worldpositionX = x * GridSizeX + GridSizeX * 0.5f;
                float worldpositionZ = z * GridSizeZ + GridSizeZ * 0.5f;

                // Update the grid position
                grid.transform.position = new Vector3(worldpositionX, ground.terrainData.GetInterpolatedHeight(worldpositionX / ground.terrainData.size.x, worldpositionZ / ground.terrainData.size.z), worldpositionZ);
                // Create Four Points that define the grid
                grid.GetComponent<Grid>().Points[0] = new Vector3(-halfGridSizeX + grid.transform.position.x, ground.SampleHeight(new Vector3(-halfGridSizeX + grid.transform.position.x, 0, -halfGridSizeZ + grid.transform.position.z)) + 2, -halfGridSizeZ + grid.transform.position.z);
                grid.GetComponent<Grid>().Points[1] = new Vector3(halfGridSizeX + grid.transform.position.x, ground.SampleHeight(new Vector3(halfGridSizeX + grid.transform.position.x, 0, -halfGridSizeZ + grid.transform.position.z))   + 2, -halfGridSizeZ + grid.transform.position.z);
                grid.GetComponent<Grid>().Points[2] = new Vector3(halfGridSizeX + grid.transform.position.x, ground.SampleHeight(new Vector3(halfGridSizeX + grid.transform.position.x, 0, halfGridSizeZ + grid.transform.position.z))    + 2, halfGridSizeZ + grid.transform.position.z);
                grid.GetComponent<Grid>().Points[3] = new Vector3(-halfGridSizeX + grid.transform.position.x, ground.SampleHeight(new Vector3(-halfGridSizeX + grid.transform.position.x, 0, halfGridSizeZ + grid.transform.position.z))  + 2, halfGridSizeZ + grid.transform.position.z);
                grid.GetComponent<Grid>().Points[4] = new Vector3(-halfGridSizeX + grid.transform.position.x, ground.SampleHeight(new Vector3(-halfGridSizeX + grid.transform.position.x, 0, -halfGridSizeZ + grid.transform.position.z)) + 2, -halfGridSizeZ + grid.transform.position.z);
                UpdateGridAvailability(grid.GetComponent<Grid>());
                if (highestYvalue == 0)
                    highestYvalue = grid.GetComponent<Grid>().Points[0].y;
                if (grid.GetComponent<Grid>().Points[0].y >= highestYvalue - 5)
                {
                    grid.GetComponent<Grid>().buildable = false;
                }
                grid.GetComponent<LineRenderer>().material = grid.GetComponent<Grid>().materials[1];
                grid.GetComponent<LineRenderer>().SetVertexCount(5);
                grid.GetComponent<LineRenderer>().SetPositions(grid.GetComponent<Grid>().Points);
                grid.GetComponent<LineRenderer>().SetWidth(1, 1);
                grid.transform.SetParent(gameObject.transform);
                gridmesh[x, z] = grid;

                grid.GetComponent<Renderer>().enabled = false;
        //        // Create a copy of the plane and offset it according to [current width, current column] using Instantiate
        //        GameObject grid = (GameObject)Instantiate(StartingGrid);
        //        grid.name = "Row: " + x + " Col: " + z;
        //        float worldpositionX = x * GridSizeX + GridSizeX * 0.5f;
        //        float worldpositionZ = z * GridSizeZ + GridSizeZ * 0.5f;
        //        grid.transform.position = new Vector3(worldpositionX, ground.terrainData.GetInterpolatedHeight(worldpositionX / ground.terrainData.size.x, worldpositionZ / ground.terrainData.size.z) + 1, worldpositionZ);
        //        grid.transform.localScale = new Vector3(GridSizeX, GridSizeZ, 1);
        //        grid.transform.SetParent(gameObject.transform);

        //        //disable grid rendering(for actual playtest)
        //        //grid.GetComponent<Renderer>().enabled = false;
        //        Vector3 terrainNormal = ground.terrainData.GetInterpolatedNormal(worldpositionX / ground.terrainData.size.x, worldpositionZ / ground.terrainData.size.z);
        //        grid.GetComponent<Grid>().position.x = x;
        //        grid.GetComponent<Grid>().position.y = z;
        //        grid.transform.rotation = Quaternion.FromToRotation(Vector3.up, terrainNormal); // ((ground.terrainData.GetInterpolatedNormal(worldpositionX / ground.terrainData.size.x, worldpositionZ / ground.terrainData.size.z)));
        //        grid.transform.Rotate(new Vector3(1, 0, 0), 90);
        //        grid.GetComponent<Grid> ().state = grid.GetComponent<Grid>().CollidedWithTerrain();
        //        grid.GetComponent<Grid>().UpdateAvailability();
        //        //, new Vector3(m_startingPlane.transform.position.x + x, m_startingPlane.transform.position.y, m_startingPlane.transform.position.z + z), m_startingPlane.transform.rotation);
        //        gridmesh[x, z] = grid;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
