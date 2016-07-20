using UnityEngine;
using UnityEngine.UI;
using VGDC_RPG.Networking;
using VGDC_RPG;
using System;
using System.Collections.Generic;
using VGDC_RPG.UI;

public class LobbyScript : MonoBehaviour, INetEventHandler
{
    public InputField msgText;
    public InputField logText;

    public Dropdown MapTypeDropdown;

    public int HandlerID { get { return -1; } }

    public List<PlayerLobbySettings> Players;

    public GameObject contentPanel;

    void Start()
    {
        if (GameLogic.Instance.IsHost && GameLogic.Instance.IsServer)
        {
            MapTypeDropdown.interactable = true;
            MatchServer.ChatReceived += MatchServer_ChatReceived;
            MatchServer.PlayerJoined += MatchServer_PlayerJoined;
            MatchServer.PlayerLeft += MatchServer_PlayerLeft;
        }
        else
            MatchClient.ChatReceived += MatchServer_ChatReceived;

        NetEvents.RegisterHandler(this);

        Players = new List<PlayerLobbySettings>();
        for (int i = 0; i < GameLogic.Instance.TeamCount; i++)
            Players.Add(new PlayerLobbySettings(contentPanel, i));

        if (GameLogic.Instance.IsHost && GameLogic.Instance.IsServer)
        {
            Players[0].SetState(true);
            Players[0].Aquire(-2);
        }
        for (int i = 1; i < GameLogic.Instance.TeamCount; i++)
            Players[i].SetState(false);
    }

    private void MatchServer_PlayerLeft(int cid)
    {
        for (int i = 1; i < GameLogic.Instance.TeamCount; i++)
            if (Players[i].CID == cid)
                Players[i].Aquire(-1);
    }

    private void MatchServer_PlayerJoined(int cid)
    {
        for (int i = 1; i < GameLogic.Instance.TeamCount; i++)
            if (Players[i].CID == -1 && Players[i].TypeDropdown.value == 0)
            {
                Players[i].Aquire(cid);

                for (int n = 0; n < GameLogic.Instance.TeamCount; n++)
                    Players[n].SyncAll();

                return;
            }
        MatchServer.Error("No open slots available!", true, MatchServer.GetConnection(cid));
    }

    void Update()
    {
        if (GameLogic.Instance.IsHost && GameLogic.Instance.IsServer)
            MatchServer.Update();
        else
            MatchClient.Update();

        foreach (var pls in Players)
            pls.Update();
    }

    private void MatchServer_ChatReceived(string msg)
    {
        logText.text += msg + '\n';
    }

    public void SendChat()
    {
        if (GameLogic.Instance.IsHost && GameLogic.Instance.IsServer)
            MatchServer.SendChat(msgText.text);
        else
            MatchClient.SendChat(msgText.text);

        msgText.text = "";
    }

    public void MapTypeChanged(int type)
    {
        if (GameLogic.Instance.IsHost && GameLogic.Instance.IsServer)
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
        if (GameLogic.Instance.IsHost && GameLogic.Instance.IsServer)
        {
            MatchServer.ChatReceived -= MatchServer_ChatReceived;
            MatchServer.PlayerJoined -= MatchServer_PlayerJoined;
            MatchServer.PlayerLeft -= MatchServer_PlayerLeft;
        }
        else
            MatchClient.ChatReceived -= MatchServer_ChatReceived;
    }

    public void HandleEvent(DataReader r)
    {
        if (!GameLogic.Instance.IsHost)
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
