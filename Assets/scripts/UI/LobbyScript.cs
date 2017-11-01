using UnityEngine;
using UnityEngine.UI;
using VGDC_RPG;
using System;
using System.Collections.Generic;
using VGDC_RPG.UI;
using VGDC_RPG.TileMapProviders;
using System.Linq;

public class LobbyScript : MonoBehaviour {
    // public InputField msgText;
    //  public InputField logText;

    public Dropdown MapTypeDropdown;

    public Button StartButton;

    public int HandlerID { get { return -1; } }

    public List<PlayerLobbySettings> Players;

    //public GameObject contentPanel;

    void Start() {
        MapTypeDropdown.interactable = true;

        StartButton.interactable = true;

        Players = new List<PlayerLobbySettings>();
        for (int i = 0; i < GameLogic.TeamCount; i++)
            Players.Add(new PlayerLobbySettings(i, -1));

        if (GameLogic.IsHost) {
            Players[0].SetState(true);
            //Players[0].Aquire(-2);
        }
        for (int i = 1; i < GameLogic.TeamCount; i++)
            Players[i].SetState(false);

        MapTypeDropdown.AddOptions(SavedTileMapProvider.GetSavedTileMaps().ToList());
    }

    public void StartGame() {
        GameLogic.PlayersCID[0] = Players[0].CID;
        GameLogic.MatchInfo.PlayerInfos[0].PlayerName = Players[0].PlayerName.text;
        GameLogic.MatchInfo.PlayerInfos[0].Team = (Byte)0;
        GameLogic.MatchInfo.PlayerInfos[0].PlayerType = GameLogic.MatchInfo.PlayerType.Local;
        for (int i = 1; i < Players.Count; i++) {
            if (Players[i].CID != -1)
                GameLogic.CIDPlayers.Add(Players[i].CID, (byte)i);
            GameLogic.PlayersCID[i] = Players[i].CID;
            GameLogic.MatchInfo.PlayerInfos[i].PlayerName = Players[i].PlayerName.text;
            GameLogic.MatchInfo.PlayerInfos[i].Team = (Byte)i;
            GameLogic.MatchInfo.PlayerInfos[i].PlayerType = GameLogic.MatchInfo.PlayerType.AI;
            GameLogic.MatchInfo.PlayerInfos[i].AIController = new VGDC_RPG.Units.AIController((byte)i);
        }
        GameLogic.StartMatch();
        if (MapTypeDropdown.value == 0)
            GameLogic.SetMapProvider(new TestTileMapProvider(32, 32));
        else if (MapTypeDropdown.value == 1)
            GameLogic.SetMapProvider(new DrunkWalkCaveProvider(32, 32));
        else
            GameLogic.SetMapProvider(new SavedTileMapProvider(MapTypeDropdown.options[MapTypeDropdown.value].text));
    }

}