using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VGDC_RPG;
using VGDC_RPG.Networking;

public class HostSetupScript : MonoBehaviour
{
    public InputField usernameField, passwordField;

    // Use this for initialization
    void Start()
    {
        new GameLogic();
        GameLogic.Instance.IsHost = true;
        GameLogic.Instance.IsServer = true;
    }

    public void BackPressed()
    {
        SceneManager.LoadScene("scenes/mainMenu");
    }

    public void StartPressed()
    {
        MatchServer.Username = usernameField.text;
        MatchServer.Init(8080, passwordField.text);
        SceneManager.LoadScene("scenes/lobby");
    }
}
