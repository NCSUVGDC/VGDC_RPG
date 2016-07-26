using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class NewMainMenuScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HostPressed()
    {
        SceneManager.LoadScene("scenes/HostSetup");
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
