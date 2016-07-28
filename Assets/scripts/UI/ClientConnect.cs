using UnityEngine;
using VGDC_RPG;
using UnityEngine.SceneManagement;
using VGDC_RPG.Networking;
using UnityEngine.UI;

public class ClientConnect : MonoBehaviour
{
    public InputField usernameField, ipField, passwordField;
    public bool connecting;

    // Use this for initialization
    void Start()
    {
        GameLogic.Init();
        GameLogic.IsHost = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (connecting)
            MatchClient.Update(1);
        if (MatchClient.Joined)
            SceneManager.LoadScene("scenes/lobby");
    }

    public void BackPressed()
    {
        SceneManager.LoadScene("scenes/newMainMenu");
    }

    public void StartPressed()
    {
        MatchClient.Init(usernameField.text);
        MatchClient.Connect(ipField.text, 8080, passwordField.text);
        connecting = true;
    }
}
