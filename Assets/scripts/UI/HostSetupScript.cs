using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VGDC_RPG;
using System;
using System.Collections.Generic;
using VGDC_RPG.UI;
using VGDC_RPG.TileMapProviders;
using System.Linq;


public class HostSetupScript : MonoBehaviour {
    public Text playerCountText;
    public Slider playerCountSlider;
    public Text aiCountText;
    public Slider aiCountSlider;
    public Dropdown MapTypeDropdown;
    public Button StartButton;

    public List<PlayerLobbySettings> Players;

    // Use this for initialization
    void Start() {
        GameLogic.Init();
        GameLogic.IsHost = true;
        MapTypeDropdown.interactable = true;
        if ((int)playerCountSlider.value + (int)aiCountSlider.value < 2) {
            StartButton.interactable = false;
        } else {
            StartButton.interactable = true;
        }

        Players = new List<PlayerLobbySettings>();
        for (int i = 0; i < GameLogic.TeamCount; i++)
            Players.Add(new PlayerLobbySettings(i, -1));
    }


    public void BackPressed() {
        SceneManager.LoadScene("scenes/newStoneSelection");
    }

    public void StartPressed() {

        // Get total teams count between player and ai teams
        Debug.Log("Start pressed on map selection");
        GameLogic.TeamCount = (int)playerCountSlider.value + (int)aiCountSlider.value;
        for (int i = 0; i < (int)playerCountSlider.value; i++) {
            Players.Add(new PlayerLobbySettings(i, 1));
            Players[i].SetState(true);
        }
        for (int i = (int)playerCountSlider.value; i < GameLogic.TeamCount; i++) {
            Players.Add(new PlayerLobbySettings(i, 2));
            Players[i].SetState(false);
        }

        MapTypeDropdown.AddOptions(SavedTileMapProvider.GetSavedTileMaps().ToList());
        StartGame();
    }

    public void PlayerCountChanged(float v) {
        playerCountText.text = v.ToString();
        if ((int)playerCountSlider.value + (int)aiCountSlider.value < 2) {
            StartButton.interactable = false;
        } else {
            StartButton.interactable = true;
        }
    }

    public void AiCountChanged(float v) {
        aiCountText.text = v.ToString();
        if ((int)playerCountSlider.value + (int)aiCountSlider.value < 2) {
            StartButton.interactable = false;
        } else {
            StartButton.interactable = true;
        }
    }

    public void StartGame() {
        for (int i = 0; i < (int)playerCountSlider.value; i++) {
            if (Players[i].CID != -1)
                GameLogic.CIDPlayers.Add(Players[i].CID, (byte)i);
            GameLogic.PlayersCID[i] = Players[i].CID;
            GameLogic.MatchInfo.PlayerInfos[i].PlayerName = Players[i].PlayerName.text;
            GameLogic.MatchInfo.PlayerInfos[i].Team = (Byte)i;
            GameLogic.MatchInfo.PlayerInfos[i].PlayerType = GameLogic.MatchInfo.PlayerType.Local;
        }

        for (int i = (int)playerCountSlider.value; i < (int)playerCountSlider.value + (int)aiCountSlider.value; i++) {
            Debug.Log("AI current i: " + i);
            if (Players[i].CID != -1)
                GameLogic.CIDPlayers.Add(Players[i].CID, (byte)i);
            GameLogic.PlayersCID[i] = Players[i].CID;
            GameLogic.MatchInfo.PlayerInfos[i].PlayerName = Players[i].PlayerName.text;
            GameLogic.MatchInfo.PlayerInfos[i].Team = (Byte)i;
            GameLogic.MatchInfo.PlayerInfos[i].PlayerType = GameLogic.MatchInfo.PlayerType.AI;
            GameLogic.MatchInfo.PlayerInfos[i].AIController = new VGDC_RPG.Units.AIController((byte)i);
        }
        Debug.Log("CIDPlayers count after initialization: " + GameLogic.CIDPlayers.Count);

        if (MapTypeDropdown.value == 0)
            GameLogic.SetMapProvider(new TestTileMapProvider(32, 32));
        else if (MapTypeDropdown.value == 1)
            GameLogic.SetMapProvider(new DrunkWalkCaveProvider(32, 32));
        else
            GameLogic.SetMapProvider(new SavedTileMapProvider(MapTypeDropdown.options[MapTypeDropdown.value].text));
        GameLogic.StartMatch();

    }
}