using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Text))]
public class FPSCounter : MonoBehaviour
{
    Text TextBox;
    public int AvgFramesToCal = 0;
    int frameCounter = 0;
    List<float> FPSstorage;
	// Use this for initialization
	void Start ()
    {
        TextBox = GetComponent<Text>();
        FPSstorage = new List<float>();
        FPSstorage.Capacity = AvgFramesToCal;
        for (int i = 0; i < AvgFramesToCal; ++i)
        {
            FPSstorage.Add(60f);
        }
        if (AvgFramesToCal == 0)
        {
            FPSstorage.Add(0f);
        }
	}

    float GetAverageFPS()
    {
        float totalFPS = 0;
        foreach (float fps in FPSstorage)
        {
            totalFPS += fps;
        }

        if (AvgFramesToCal == 0)
        {
            return totalFPS / 1;
        }
        else 
        {
            return totalFPS / AvgFramesToCal;
        }

    }

	// Update is called once per frame
	void Update ()
    {
        FPSstorage[frameCounter] = 1f / Time.deltaTime;
        ++frameCounter;

        if (frameCounter >= AvgFramesToCal)
        {
            frameCounter = 0;
        }

        TextBox.text = "FPS: " + GetAverageFPS();
	}
}
