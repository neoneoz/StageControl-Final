using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FogOfWar : MonoBehaviour
{
    // Singleton
    static public FogOfWar instance = null;

    // Ratio World : Texture for x to x and z to y axis
    private Vector2 WorldToTextureRatio = new Vector2();

    // Height from the ground
    public float quadHeight = 180;
    float defaultquadHeight;

    // Fog Color
    public Color fogColor = Color.black;

    // Object Lists
    public GameObject EntityList;
    public GameObject Buildings;

   // Texture to read and write to
    public Texture2D FogTexture;

    // Circle Curve, how transparent it is to the edge
    public AnimationCurve FogCurve;

    // Circle Storage, used to store circles to not draw more than once
    public Dictionary<int, Color[]> Circles = new Dictionary<int,Color[]>();
    MeshRenderer Plane;
    GameObject CollisionPlane;
    Color[] ClearScreen;

    //Optimization Codes
    bool ListsSet = false;
    public List<GameObject> allObjects = new List<GameObject>();
    int ObjIndex = 0;
    public int NumToUpdate = 10;

    // Objects in Range
    static Dictionary<uint, GameObject> EnemiesInRange = new Dictionary<uint, GameObject>();
    static Dictionary<uint, GameObject> FriendsInRange = new Dictionary<uint, GameObject>();

    Color[] DrawCircle(int radius)
    {
        Color[] circle;
        int x, y;
        int diameter = radius + radius;

        circle = new Color[diameter * diameter];
        for (x = 0; x < diameter; x++)
        {
            for (y = 0; y < diameter; y++)
            {
                float distanceToCenter = Mathf.Sqrt((radius - x) * (radius - x) + (radius - y) * (radius - y));
                float normalizedDistance = distanceToCenter / radius;
                int index = y + (diameter * x);
                circle[index] = Color.white * (1f - FogCurve.Evaluate (1f - normalizedDistance));
            }
        }
        return circle;
    }

    void ConvertWorldPosToTexturePos(Vector3 Position, out int x, out int y)
    {
        Position.x /= SceneData.sceneData.ground.terrainData.size.x;
        Position.z /= SceneData.sceneData.ground.terrainData.size.z;
        Position.x *= FogTexture.width;
        Position.z *= FogTexture.height;

        x = (int)Position.x;
        y = (int)Position.z;
    }

    int ConvertWorldScaleToTextureScale(float vision)
    {
        return (int)(vision * WorldToTextureRatio.x);
    }

	void Start ()
    {
        if (instance == null)
        {
            instance = this;
        }
        else 
        {
            Destroy(this);
        }
        CollisionPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        CollisionPlane.transform.Rotate(Vector3.up, 180f);
        CollisionPlane.transform.Rotate(Vector3.right, 180f);

        defaultquadHeight = quadHeight;
        WorldToTextureRatio.x = FogTexture.width / SceneData.sceneData.ground.terrainData.size.x;
        WorldToTextureRatio.y = FogTexture.height / SceneData.sceneData.ground.terrainData.size.z;

        Plane = GetComponent<MeshRenderer>();
        transform.position = new Vector3(SceneData.sceneData.ground.terrainData.size.x * 0.5f, quadHeight, SceneData.sceneData.ground.terrainData.size.z * 0.5f);
        transform.localScale = new Vector3(SceneData.sceneData.ground.terrainData.size.x * 0.1f, 1, SceneData.sceneData.ground.terrainData.size.z * 0.1f);

        CollisionPlane.transform.position = new Vector3(SceneData.sceneData.ground.terrainData.size.x * 0.5f, quadHeight, SceneData.sceneData.ground.terrainData.size.z * 0.5f);
        CollisionPlane.transform.localScale = new Vector3(SceneData.sceneData.ground.terrainData.size.x * 0.1f, 1, SceneData.sceneData.ground.terrainData.size.z * 0.1f);
        Destroy(CollisionPlane.GetComponent<MeshRenderer>());

        ClearScreen = new Color[FogTexture.width * FogTexture.height];

        for(int x = 0; x < FogTexture.width; ++x)
        {
            for (int y = 0; y < FogTexture.height; ++y)
            {
                ClearScreen[y + (FogTexture.width * x)] = fogColor;
            }
        }
	}

    void AddFogAt(int x, int y, int radius)
    {
        int StartX = Mathf.Clamp(x - radius, 0, x - radius);
        int StartY = Mathf.Clamp(y - radius, 0, y - radius);
        int diameter = radius + radius;
        Color[] currentColor = FogTexture.GetPixels(StartX, StartY, diameter, diameter);
        if (!Circles.ContainsKey(radius))
        {
            Circles.Add(radius, DrawCircle(radius));
        }

        for (int i = 0; i < Circles[radius].Length; ++i)
        {
            currentColor[i] *= Circles[radius][i];
        }

        FogTexture.SetPixels(StartX, StartY, diameter, diameter, currentColor);
    }

    void SetList()
    {
        FogTexture.SetPixels(0, 0, FogTexture.width, FogTexture.height, ClearScreen);
        allObjects.Clear();
        allObjects.AddRange(Building.m_buildingList);

        for (int EntityIndex = 0; EntityIndex < SceneData.sceneData.EntityList.transform.childCount; ++EntityIndex)
        {
            allObjects.Add(SceneData.sceneData.EntityList.transform.GetChild(EntityIndex).gameObject);
        }

        ObjIndex = 0;
        ListsSet = true;
    }

    bool CheckIfFriendly(GameObject obj)
    {
        if(obj.GetComponent<Unit>())
        {
            return obj.GetComponent<Unit>().m_isFriendly;
        }else if(obj.GetComponent<Building>())
        {
            return obj.GetComponent<Building>().isfriendly;
        }
        return false;
    }

    void UpdateTexture()
    {
        for (int i = 0; i < NumToUpdate; ++i)
        {
            if (!allObjects[ObjIndex])
            {
                ++ObjIndex;
                if (ObjIndex >= allObjects.Count - 1)
                {
                    ListsSet = false;
                    break;
                }
                continue;
            }
            if (!allObjects[ObjIndex].activeSelf)
            {
                ++ObjIndex;
                continue;
            }

                if (allObjects[ObjIndex].GetComponent<Vision>() && CheckIfFriendly(allObjects[ObjIndex]))
                {
                    int radius = ConvertWorldScaleToTextureScale(allObjects[ObjIndex].GetComponent<Vision>().radius);
                    Ray ray = new Ray(allObjects[ObjIndex].transform.position, (SceneData.sceneData.camera.transform.position - allObjects[ObjIndex].transform.position).normalized);
                    RaycastHit hit;
                    CollisionPlane.GetComponent<MeshCollider>().Raycast(ray, out hit, 1000);
                    int x = 0;
                    int y = 0;
                    ConvertWorldPosToTexturePos(hit.point, out x, out y);
                    if (hit.collider != null)
                    {
                        AddFogAt(x, y, radius);
                    }
                }

                if (ObjIndex >= allObjects.Count - 1)
                {
                    ListsSet = false;
                    break;
                }
                ++ObjIndex;
        }

        //for (int buildingIndex = 0; buildingIndex < Building.m_buildingList.Count; ++buildingIndex)
        //{
        //    if (Building.m_buildingList[buildingIndex].GetComponent<Vision>())
        //    {
        //        int radius = ConvertWorldScaleToTextureScale(Building.m_buildingList[buildingIndex].GetComponent<Vision>().radius);
        //        Ray ray = new Ray(Building.m_buildingList[buildingIndex].transform.position, (SceneData.sceneData.camera.transform.position - Building.m_buildingList[buildingIndex].transform.position).normalized);
        //        RaycastHit hit;
        //        Plane.GetComponent<MeshCollider>().Raycast(ray, out hit, 1000);
        //        int x = 0;
        //        int y = 0;
        //        ConvertWorldPosToTexturePos(hit.point, out x, out y);
        //        if (hit.collider != null)
        //        {
        //            AddFogAt(x, y, radius);
        //        }
        //    }
        //}

        //for (int EntityIndex = 0; EntityIndex < SceneData.sceneData.EntityList.transform.childCount; ++EntityIndex)
        //{
        //    if (SceneData.sceneData.EntityList.transform.GetChild(EntityIndex).GetComponent<Vision>())
        //    {
        //        int radius = ConvertWorldScaleToTextureScale(SceneData.sceneData.EntityList.transform.GetChild(EntityIndex).GetComponent<Vision>().radius);
        //        Ray ray = new Ray(SceneData.sceneData.EntityList.transform.GetChild(EntityIndex).transform.position, (SceneData.sceneData.camera.transform.position - SceneData.sceneData.EntityList.transform.GetChild(EntityIndex).transform.position).normalized);
        //        RaycastHit hit;
        //        Plane.GetComponent<MeshCollider>().Raycast(ray, out hit, 1000);
        //        int x = 0;
        //        int y = 0;
        //        ConvertWorldPosToTexturePos(hit.point, out x, out y);
        //        if (hit.collider != null)
        //        {
        //            AddFogAt(x, y, radius);
        //        }
        //    }
        //}
        FogTexture.Apply();  
    }

    public Dictionary<uint, GameObject> GetInRangeEnemies(bool isFriendly)
    {
        if (isFriendly)
            return EnemiesInRange;
        else
            return FriendsInRange;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (ListsSet == false)
        {
            SetList();
        }
            UpdateTexture();
	}

    void OnDestroy()
    {
        FogTexture.SetPixels(0, 0, FogTexture.width, FogTexture.height, ClearScreen);
        FogTexture.Apply();
    }
}
