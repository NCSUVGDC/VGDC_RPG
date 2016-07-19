using UnityEngine;
using UnityEngine.SceneManagement;
using VGDC_RPG;
using VGDC_RPG.Networking;

public class HostSetupScript : MonoBehaviour
{
    public UnityEngine.UI.Text passwordField;

    // Use this for initialization
    void Start()
    {
        new GameLogic();
        GameLogic.Instance.IsHost = true;
        GameLogic.Instance.IsServer = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BackPressed()
    {
        SceneManager.LoadScene("scenes/mainMenu");
    }

    public void StartPressed()
    {
        MatchServer.Init(8080, passwordField.text);
        SceneManager.LoadScene("scenes/lobby");
    }
}
