using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour {
    public string m_sceneName;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void GoToScene()
    {
        SceneManager.LoadScene(m_sceneName);
    }

    public static void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void GoToLevel1()
    {
#if UNITY_ANDROID
        SceneManager.LoadScene("Stage 1 Android");
#else
        SceneManager.LoadScene("Stage 1 PC");
#endif
    }

    public void GoToLevel2()
    {
#if UNITY_ANDROID
        SceneManager.LoadScene("Stage 2 Android");
#else
        SceneManager.LoadScene("Stage 2 PC");
#endif
    }

}
