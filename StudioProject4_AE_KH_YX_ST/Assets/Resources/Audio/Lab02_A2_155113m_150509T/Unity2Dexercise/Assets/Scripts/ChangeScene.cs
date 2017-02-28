using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ChangeScene : MonoBehaviour {

    Scene currentScene;
    public Scene prevScene;
    public string sceneString = " ";

	// Use this for initialization
	void Start () {
        currentScene = SceneManager.GetActiveScene();
        
	}
	
	// Update is called once per frame
	void Update () {
        if (sceneString != " ")
        {
            prevScene = currentScene;
            SceneManager.LoadScene(sceneString, LoadSceneMode.Single);
            sceneString = " ";
        }
	}

    public void GetScene(string sceneName)
    {
        sceneString = sceneName;
    }
}
