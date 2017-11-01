using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VGDC_RPG;
using VGDC_RPG.UI;
using System;
using VGDC_RPG.TileMapProviders;

public class mapSelectScript : MonoBehaviour {

    public Button startButton;

    public Toggle team1p;
    public Toggle team1ai;

    public Toggle team2p;
    public Toggle team2ai;

    public Toggle map1;
    public Toggle map2;

    public ToggleGroup team1group;
    public ToggleGroup team2group;
    public ToggleGroup mapgroup;
    public List<PlayerLobbySettings> Players;

    public int playerCount;
    public int aiCount;

    public Image mapPreview;
    public Text mapPreviewQuestionMark;

    public void Start() {
        playerCount = 0;
        aiCount = 0;
        startButton.interactable = false;
        GameLogic.Init();
        GameLogic.IsHost = true;
        Players = new List<PlayerLobbySettings>();
    }

    public void Update() {
        if(startButton.interactable == true) {
            return;
        } else if (team1group.AnyTogglesOn() && team2group.AnyTogglesOn() && mapgroup.AnyTogglesOn()) {
            startButton.interactable = true;
        }

        Debug.Log("Player Count: " + playerCount + ", Ai Count: " + aiCount);
    }
    public void backClicked() {
        GameLogic.reset();
        SceneManager.LoadScene("scenes/newStoneSelection");
    }

    public void startClicked() {
        GameLogic.TeamCount = playerCount + aiCount;
        for (int i = 0; i < playerCount; i++) {
            Players.Add(new PlayerLobbySettings(i, 1));
            Players[i].SetState(true);
        }
        for (int i = playerCount; i < GameLogic.TeamCount; i++) {
            Players.Add(new PlayerLobbySettings(i, 2));
            Players[i].SetState(false);
        }


        StartGame();  
    }


    public void StartGame() {
        for (int i = 0; i < playerCount; i++) {
            if (Players[i].CID != -1)
                GameLogic.CIDPlayers.Add(Players[i].CID, (byte)i);
            GameLogic.PlayersCID[i] = Players[i].CID;
            GameLogic.MatchInfo.PlayerInfos[i].PlayerName = Players[i].PlayerName.text;
            GameLogic.MatchInfo.PlayerInfos[i].Team = (Byte)i;
            GameLogic.MatchInfo.PlayerInfos[i].PlayerType = GameLogic.MatchInfo.PlayerType.Local;
        }

        for (int i = playerCount; i < playerCount + aiCount; i++) {
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

        if (map1.isOn && !map2.isOn) {
            GameLogic.SetMapProvider(new TestTileMapProvider(32, 32));
        } else if (!map1.isOn && map2.isOn) {
            GameLogic.SetMapProvider(new DrunkWalkCaveProvider(32, 32));
        } else {
            GameLogic.reset();
            SceneManager.LoadScene("scenes/newStoneSelection");
        }
        GameLogic.StartMatch();
    }
}
