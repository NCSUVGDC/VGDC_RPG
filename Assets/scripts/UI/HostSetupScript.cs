﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VGDC_RPG;
using VGDC_RPG.Networking;

public class HostSetupScript : MonoBehaviour
{
    public InputField usernameField, passwordField;
    public Text playerCountText;
    public Slider playerCountSlider;

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
        MatchServer.MaxConnections = (int)playerCountSlider.value;
        GameLogic.Instance.TeamCount = (int)playerCountSlider.value;
        MatchServer.Username = usernameField.text;
        MatchServer.Init(8080, passwordField.text);
        SceneManager.LoadScene("scenes/lobby");
    }

    public void PlayerCountChanged(float v)
    {
        playerCountText.text = v.ToString();
    }
}
