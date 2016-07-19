using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    void OnGUI()
    {
        var buttonWidth = 200;
        var buttonHeight = 30;
        if (GUI.Button(new Rect((Screen.width - buttonWidth) / 2, Screen.height / 2 - buttonHeight, buttonWidth, buttonHeight), "Start"))
            SceneManager.LoadScene("scenes/mapTestScene");
        else if (GUI.Button(new Rect((Screen.width - buttonWidth) / 2, Screen.height / 2, buttonWidth, buttonHeight), "Screen Settings"))
            SceneManager.LoadScene("scenes/screenSettings");

        else if (GUI.Button(new Rect((Screen.width - buttonWidth) / 2, Screen.height / 2 + 2 * buttonHeight, buttonWidth, buttonHeight), "Exit"))
            Application.Quit();
    }
}
