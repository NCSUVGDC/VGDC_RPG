using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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

	public void mouseOver(GameObject t) {
        if(t.GetComponent<Text>()) {
            t.GetComponent<Text>().color = Color.grey;
        } else if (t.GetComponent<Image>()) {
            t.GetComponent<Image>().color = Color.grey;
        }
    }

    public void mouseOut(GameObject t) {
        if (t.GetComponent<Text>()) {
            t.GetComponent<Text>().color = Color.white;
        } else if (t.GetComponent<Image>()) {
            t.GetComponent<Image>().color = Color.white;
        }
    }
}
