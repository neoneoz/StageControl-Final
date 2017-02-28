using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpatialPartition : MonoBehaviour
{
    public static SpatialPartition instance = null;
    public bool CreateDebugTiles = false;
    List<GameObject> MigrationList = new List<GameObject>();
    public List<SPGrid> SPGridMesh;
    List<GameObject> toBeRemoved = new List<GameObject>();

    public int GridSizeX = 100;
    public int GridSizeZ = 100;
    int m_rows = 0;
    int m_columns = 0;

    void GenerateSpatialPartition()
    {
        m_rows = (int)SceneData.sceneData.ground.terrainData.size.x / GridSizeX;
        m_columns = (int)SceneData.sceneData.ground.terrainData.size.z / GridSizeZ;

        SPGridMesh = new List<SPGrid>();

        for (int x = 0; x < m_rows; ++x)
        {
            for (int y = 0; y < m_columns; ++y)
            {
                    GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    temp.transform.Rotate(Vector3.right, 90);
                    temp.transform.localScale = new Vector3(GridSizeX, GridSizeZ, 1);
                    temp.transform.position = new Vector3(x * GridSizeX + (GridSizeX * 0.5f), 500, y * GridSizeZ + (GridSizeZ * 0.5f));
                    temp.transform.SetParent(this.transform);
                    temp.AddComponent<SPGrid>();
                    temp.GetComponent<SPGrid>().SetPos(x, y);
                    SPGridMesh.Add(temp.GetComponent<SPGrid>());
                if (!CreateDebugTiles)
                {
                    Destroy(temp.GetComponent<MeshRenderer>());
                }
            }
        }
    }

	// Use this for initialization
	void Start ()
    {
        if (instance == null)
        {
            instance = this;
        }

        GenerateSpatialPartition();
	}

    uint GetID(GameObject obj)
    {
        if (obj.GetComponent<Unit>())
        {
            return obj.GetComponent<Unit>().GetID();
        }
        else if (obj.GetComponent<Building>())
        {
            return obj.GetComponent<Building>().GetID();
        }

        Debug.Log("Object does not have Unit/Building Component");
        Debug.Break();

        return 0;
    }

    void CheckForMigration()
    {
        for (int x = 0; x < m_rows; ++x)
        {
            for (int y = 0; y < m_columns; ++y)
            {
                foreach (GameObject obj in SPGridMesh[x * m_columns + y].ObjectList)
                //for (int i = 0; i < SPGridMesh[x * m_columns + y].ObjectList.Values.Count;)
                //for (int i = 0; i < SPGridMesh[x, y].ObjectList.Count; ++i)
                {
                    if (!obj)
                    {
                        toBeRemoved.Add(obj);
                        continue;

                    }

                    if (obj.transform.position.x < (x * GridSizeX) ||
                        obj.transform.position.x > (x * GridSizeX) + GridSizeX ||
                        obj.transform.position.z < (y * GridSizeZ)  ||
                        obj.transform.position.z > (y * GridSizeZ) + GridSizeZ)
                    {
                        toBeRemoved.Add(obj);
                        MigrationList.Add(obj);
                    }
                }

                foreach (GameObject obj in toBeRemoved)
                {
                    SPGridMesh[x * m_columns + y].ObjectList.Remove(obj);
                }
                toBeRemoved.Clear();
            }
        }
    }

    void MigrationUpdate()
    {
        while (MigrationList.Count > 0)
        {
            int index_X = (int)(MigrationList[0].transform.position.x / GridSizeX);
            int index_Y = (int)(MigrationList[0].transform.position.z / GridSizeZ);
            //Debug.Log("Index X: " + index_X + " Index Y: " + index_Y);
            SPGridMesh[index_X * m_columns + index_Y].AddObject(MigrationList[0]);
            MigrationList.Remove(MigrationList[0]);
        }
    }
	// Update is called once per frame
	void Update ()
    {
        CheckForMigration();
        MigrationUpdate();
	}

    public void AddGameObject(GameObject obj)
    {
        int index_X = (int)(obj.transform.position.x / GridSizeX);
        int index_Y = (int)(obj.transform.position.z / GridSizeZ);

        SPGridMesh[index_X * m_columns + index_Y].AddObject(obj);
    }

    public void RemoveGameObject(GameObject obj)
    {
        int index_X = (int)(obj.transform.position.x / GridSizeX);
        int index_Y = (int)(obj.transform.position.z / GridSizeZ);

        if (index_X < 0 || index_Y < 0
        || index_X > m_rows - 1|| index_Y > m_columns - 1)
        {
            return;
        }

        SPGridMesh[index_X * m_columns + index_Y].Remove(obj);
    }

    public List<GameObject> GetObjectListAt(Vector3 Position)
    {
        int index_X = (int)(Position.x / GridSizeX);
        int index_Y = (int)(Position.z / GridSizeZ);

        return SPGridMesh[index_X * m_columns + index_Y].ObjectList;
    }

    public List<GameObject> GetObjectListAt(Vector3 Position, float range)
    {
        int min_indexX = (int)(Position.x - range / GridSizeX);
        int min_indexY = (int)(Position.z - range / GridSizeZ);

        int max_indexX = (int)(Position.x + range / GridSizeX);
        int max_indexY = (int)(Position.z + range / GridSizeZ);

        if (min_indexX < 0)
            min_indexX = 0;
        if (min_indexY < 0)
            min_indexY = 0;

        if (max_indexX > m_rows - 1)
            max_indexX = m_rows - 1;
        if (max_indexY < m_columns - 1)
            max_indexY = m_columns - 1;

        List<GameObject> NearbyList = new List<GameObject>();

        for (int index_X = min_indexX; index_X < max_indexX; ++index_X)
        {
            for (int index_Y = min_indexY; index_Y < max_indexY; ++index_Y)
            {
                NearbyList.AddRange(SPGridMesh[index_X * m_columns + index_Y].ObjectList);
            }
        }

        return NearbyList;
    }

    public List<GameObject> GetObjectListAt(Vector3 Position, float range, out int minx, out int miny, out int maxx, out int maxy)
    {
        int min_indexX = (int)((Position.x - range) / GridSizeX);
        int min_indexY = (int)((Position.z - range) / GridSizeZ);

        int max_indexX = (int)((Position.x + range) / GridSizeX);
        int max_indexY = (int)((Position.z + range) / GridSizeZ);

        if (min_indexX < 0)
            min_indexX = 0;
        if (min_indexY < 0)
            min_indexY = 0;

        if (max_indexX > m_rows - 1)
            max_indexX = m_rows - 1;
        if (max_indexY < m_columns - 1)
            max_indexY = m_columns - 1;

        minx = min_indexX;
        miny = min_indexY;

        maxx = max_indexX;
        maxy = max_indexY;

        List<GameObject> NearbyList = new List<GameObject>();

        for (int index_X = min_indexX; index_X <max_indexX; ++index_X)
        {
            for (int index_Y = min_indexY; index_Y < max_indexY; ++index_Y)
            {

				if (index_X < 0 || index_Y < 0
					|| index_X > m_rows - 1|| index_Y > m_columns - 1)
				{
					int a = 0;
				}
                NearbyList.AddRange(SPGridMesh[index_X * m_columns + index_Y].ObjectList);
            }
        }

        return NearbyList;
    }
}
