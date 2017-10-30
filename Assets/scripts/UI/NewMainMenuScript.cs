using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using VGDC_RPG;

public class NewMainMenuScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        GameLogic.reset();
    }

    public void HostPressed()
    {
        SceneManager.LoadScene("scenes/newStoneSelection");
    }

    public void JoinPressed()
    {
        SceneManager.LoadScene("scenes/ClientConnect");
    }

    public void SettingsPressed()
    {
        SceneManager.LoadScene("scenes/screenSettings");
    }

    public void ExitPressed()
    {
        Application.Quit();
    }
}
