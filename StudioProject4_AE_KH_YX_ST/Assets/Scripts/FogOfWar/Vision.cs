using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Vision : MonoBehaviour
{
    public int radius = 100;
    void Start()
    { 
#if UNITY_ANDROID
        Destroy(GetComponent<Vision>());
#endif
    }
}
