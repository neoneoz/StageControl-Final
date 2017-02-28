using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SPGrid : MonoBehaviour
{
    public void SetPos(int x, int y)
    {
        xPos = x;
        yPos = y;
    }

    public List<GameObject> ObjectList = new List<GameObject>();
    public int xPos = 0;
    public int yPos = 0;

    public void AddObject(GameObject obj)
    {
        ObjectList.Add(obj);
    }

    public void Remove(GameObject obj)
    {
        ObjectList.Remove(obj);
    }
}
