using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScaleTransition : MonoBehaviour {

    public bool startHidden = true;
    public float disappearSize = 0.1f;
    public float popSpeed = 1.0f;

    private bool shouldShow;
    private RectTransform recty;

	// Use this for initialization
    void Awake()
    {
        recty = GetComponentInParent<RectTransform>();

        if (startHidden)
        {
            shouldShow = true;
            gameObject.SetActive(true);
            recty.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            shouldShow = false;
            gameObject.SetActive(false);
            recty.localScale = new Vector3(disappearSize, disappearSize, 1);
        }
	}
	
	// Update is called once per frame
	void Update () 
    {float dt = Time.deltaTime;

        if(shouldShow)
        {
            if (recty.localScale.x < 1)
            {
                Vector3 nextSize = recty.localScale + new Vector3(popSpeed * dt, popSpeed * dt, 0);
                if(nextSize.x > 1)
                {
                    nextSize.x = 1;
                    nextSize.y = 1;
                }

                recty.localScale = nextSize;
            }
        }
        else
        {
            //if more than de-spawn size, scale down
            if (recty.localScale.x > disappearSize)
            {
                Vector3 nextSize = recty.localScale - new Vector3(popSpeed * dt, popSpeed * dt, 0);
                if (nextSize.x < disappearSize)
                {
                    nextSize.x = disappearSize;
                    nextSize.y = disappearSize;
                }

                recty.localScale = nextSize;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
	}

    public void Enter()
    {
        gameObject.SetActive(true);
        shouldShow = true;
    }

    public void Exit()
    {
        shouldShow = false;
    }

    public void Toggle()
    {
        shouldShow = !shouldShow;
        if (shouldShow)
            Enter();
        else
            Exit();
    }
}
