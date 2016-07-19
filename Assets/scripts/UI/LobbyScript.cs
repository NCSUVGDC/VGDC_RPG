using UnityEngine;
using System.Collections;
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
        }
        else
            MatchClient.ChatReceived += MatchServer_ChatReceived;

        NetEvents.RegisterHandler(this);

        Players = new List<PlayerLobbySettings>();
        for (int i = 0; i < GameLogic.Instance.TeamCount; i++)
            Players.Add(new PlayerLobbySettings(contentPanel, i));

        Players[0].SetState(true);
        for (int i = 1; i < GameLogic.Instance.TeamCount; i++)
            Players[i].SetState(false);
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
            MatchServer.ChatReceived -= MatchServer_ChatReceived;
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
