﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OnCollision : MonoBehaviour {


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.name == "button_b")
        {
            Destroy(other.gameObject);
        }
    }
}
