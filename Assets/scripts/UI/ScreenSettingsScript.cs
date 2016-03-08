using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScreenSettingsScript : MonoBehaviour {

    Resolution[] res;
    bool fullscreen;
    bool vsync;

	// Use this for initialization
	void Start () {
        res = Screen.resolutions;
        fullscreen = Screen.fullScreen;
        vsync = QualitySettings.vSyncCount == 1;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    
    void OnGUI()
    {
        for (int i = 0; i < res.Length; i++)
        {
            if (GUI.Button(new Rect(0, i * 30, 200, 30), res[i].width + "x" + res[i].height + "@" + res[i].refreshRate + "hz"))
                Screen.SetResolution(res[i].width, res[i].height, fullscreen, res[i].refreshRate);
        }
        if (GUI.Toggle(new Rect(200, 0, 200, 30), fullscreen, "Fullscreen") != fullscreen)
            {
                fullscreen = !fullscreen;
                Screen.fullScreen = fullscreen;
            }
        if (GUI.Toggle(new Rect(200, 30, 200, 30), fullscreen, "V-Sync") != vsync)
        {
            vsync = !vsync;
            QualitySettings.vSyncCount = vsync ? 1 : 0;
        }
        if (GUI.Button(new Rect(Screen.width - 100, Screen.height - 30, 100, 30), "Back"))
            SceneManager.LoadScene("mainMenu");
    }
}
