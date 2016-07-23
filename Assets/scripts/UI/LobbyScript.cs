using UnityEngine;
using UnityEngine.UI;
using VGDC_RPG.Networking;
using VGDC_RPG;
using System;
using System.Collections.Generic;
using VGDC_RPG.UI;
using VGDC_RPG.TileMapProviders;

public class LobbyScript : MonoBehaviour, INetEventHandler
{
    public InputField msgText;
    public InputField logText;

    public Dropdown MapTypeDropdown;

    public Button StartButton;

    public int HandlerID { get { return -1; } }

    public List<PlayerLobbySettings> Players;

    public GameObject contentPanel;

    void Start()
    {
        if (GameLogic.IsHost)
        {
            MapTypeDropdown.interactable = true;
            MatchServer.ChatReceived += MatchServer_ChatReceived;
            MatchServer.PlayerJoined += MatchServer_PlayerJoined;
            MatchServer.PlayerLeft += MatchServer_PlayerLeft;
        }
        else
        {
            MatchClient.ChatReceived += MatchServer_ChatReceived;
            StartButton.gameObject.SetActive(false);
        }

        NetEvents.RegisterHandler(this);

        Players = new List<PlayerLobbySettings>();
        for (int i = 0; i < GameLogic.TeamCount; i++)
            Players.Add(new PlayerLobbySettings(contentPanel, i));

        if (GameLogic.IsHost)
        {
            Players[0].SetState(true);
            Players[0].Aquire(-2);
        }
        for (int i = 1; i < GameLogic.TeamCount; i++)
            Players[i].SetState(false);
    }

    private void MatchServer_PlayerLeft(int cid)
    {
        for (int i = 1; i < GameLogic.TeamCount; i++)
            if (Players[i].CID == cid)
                Players[i].Aquire(-1);
    }

    private void MatchServer_PlayerJoined(int cid)
    {
        for (int i = 1; i < GameLogic.TeamCount; i++)
            if (Players[i].CID == -1 && Players[i].TypeDropdown.value == 0)
            {
                Players[i].Aquire(cid);

                // Sync all items
                for (int n = 0; n < GameLogic.TeamCount; n++)
                    Players[n].SyncAll();
                MapTypeChanged(MapTypeDropdown.value);

                return;
            }

        MatchServer.Error("No open slots available!", true, MatchServer.GetConnection(cid));
    }

    void Update()
    {
        if (GameLogic.IsHost)
            MatchServer.Update();
        else
            MatchClient.Update();

        bool ready = true;
        foreach (var pls in Players)
        {
            pls.Update();
            if (!pls.ReadyToggle.isOn && pls.CID != -1)
                ready = false;
        }
        if (GameLogic.IsHost)
            if (StartButton.interactable != ready)
                StartButton.interactable = ready;
    }

    private void MatchServer_ChatReceived(string msg)
    {
        logText.text += msg + '\n';
    }

    public void SendChat()
    {
        if (GameLogic.IsHost)
            MatchServer.SendChat(msgText.text);
        else
            MatchClient.SendChat(msgText.text);

        msgText.text = "";
    }

    public void StartGame()
    {
        if (GameLogic.IsHost)
        {
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].CID != -1)
                    GameLogic.CIDPlayers.Add(Players[i].CID, (byte)i);
                GameLogic.PlayersCID[i] = Players[i].CID;
                GameLogic.MatchInfo.PlayerInfos[i].PlayerName = Players[i].PlayerName.text;
                GameLogic.MatchInfo.PlayerInfos[i].Team = (byte)Players[i].TeamDropdown.value;
            }

            GameLogic.StartMatch();
            if (MapTypeDropdown.value == 0)
                GameLogic.SetMap(new TestTileMapProvider(32, 32).GetTileMap());
            else if (MapTypeDropdown.value == 1)
                GameLogic.SetMap(new DrunkWalkCaveProvider(32, 32).GetTileMap());
            else
                throw new Exception("Invalid map type.");
        }
    }

    public void MapTypeChanged(int type)
    {
        if (GameLogic.IsHost)
        {
            var w = new DataWriter(16);
            w.Write((byte)NetCodes.Event);
            w.Write(HandlerID);
            w.Write((byte)EventType.MapTypeChanged);
            w.Write(type);

            MatchServer.Send(w);
        }
    }

    void OnDestroy()
    {
        if (GameLogic.IsHost)
        {
            MatchServer.ChatReceived -= MatchServer_ChatReceived;
            MatchServer.PlayerJoined -= MatchServer_PlayerJoined;
            MatchServer.PlayerLeft -= MatchServer_PlayerLeft;
        }
        else
            MatchClient.ChatReceived -= MatchServer_ChatReceived;
    }

    public void HandleEvent(int cid, DataReader r)
    {
        if (!GameLogic.IsHost)
        {
            var et = (EventType)r.ReadByte();

            switch (et)
            {
                case EventType.MapTypeChanged:
                    MapTypeDropdown.value = r.ReadInt32();
                    break;
                default:
                    throw new Exception("Invalid event type: " + et.ToString());
            }
        }
    }

    private enum EventType : byte
    {
        ERROR = 0,
        MapTypeChanged,
    }
}
