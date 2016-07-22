using System;
using UnityEngine;
using UnityEngine.UI;
using VGDC_RPG.Networking;

namespace VGDC_RPG.UI
{
    public class PlayerLobbySettings : INetEventHandler
    {
        public Text PlayerName;
        public Dropdown TypeDropdown;
        public Dropdown TeamDropdown;
        public Toggle ReadyToggle;
        public GameObject pip;

        private static int nid = -20;

        public int CID { get; private set; }

        public int HandlerID { get; private set; }

        public PlayerLobbySettings(GameObject content, int i)
        {
            CID = -1;

            pip = UnityEngine.Object.Instantiate(Resources.Load("PlayerInfoPanel") as GameObject);
            pip.transform.SetParent(content.transform);
            PlayerName = pip.transform.FindChild("Text").GetComponent<Text>();
            TypeDropdown = pip.transform.FindChild("Dropdown").GetComponent<Dropdown>();
            TeamDropdown = pip.transform.FindChild("TeamDropdown").GetComponent<Dropdown>();
            ReadyToggle = pip.transform.FindChild("Toggle").GetComponent<Toggle>();

            ReadyToggle.isOn = false;

            HandlerID = nid--;

            pip.GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(0, -50 * i);

            content.GetComponent<RectTransform>().sizeDelta = new Vector2(content.GetComponent<RectTransform>().sizeDelta.x, i * 50 + 50);

            NetEvents.RegisterHandler(this);

            ltd = TypeDropdown.value;
            lts = TeamDropdown.value;
            lrs = ReadyToggle.isOn;
        }

        public void HandleEvent(int cid, DataReader r)
        {
            //if (!GameLogic.Instance.IsHost)
            {
                var et = (EventType)r.ReadByte();

                switch (et)
                {
                    case EventType.ReadyOn:
                        ReadyToggle.isOn = true;
                        lrs = true;
                        break;
                    case EventType.ReadyOff:
                        ReadyToggle.isOn = false;
                        lrs = false;
                        break;
                    case EventType.TypeChanged:
                        TypeDropdown.value = r.ReadInt32();
                        ltd = TypeDropdown.value;
                        break;
                    case EventType.TeamChanged:
                        TeamDropdown.value = r.ReadInt32();
                        lts = TeamDropdown.value;
                        break;
                    case EventType.NameChanged:
                        PlayerName.text = r.ReadString();
                        break;
                    case EventType.Aquire:
                        SetState(true);
                        break;
                    default:
                        throw new Exception("Invalid event type: " + et.ToString());
                }
            }
        }

        public void SetState(bool mine)
        {
            if (GameLogic.IsHost)
            {
                TypeDropdown.interactable = true;
            }
            if (mine)
            {
                ReadyToggle.interactable = true;
                TeamDropdown.interactable = true;
            }
        }

        public void Aquire(int cid)
        {
            this.CID = cid;
            if (cid >= 0)
            {
                PlayerName.text = (string)MatchServer.GetConnection(cid).Tag;
                TypeDropdown.interactable = false;
                TeamDropdown.interactable = false;
                SendNameEvent();
                SendAquireEvent(cid);
            }
            else if (cid == -1)
            {
                PlayerName.text = "Empty";
                TypeDropdown.interactable = true;
                TeamDropdown.interactable = false;

                SendNameEvent();
            }
            else if (cid == -2)
            {
                PlayerName.text = MatchServer.Username;
                TypeDropdown.interactable = false;
                TeamDropdown.interactable = true;

                SendNameEvent();
            }
        }

        private void SendAquireEvent(int cid)
        {
            var w = new DataWriter(256);
            w.Write((byte)NetCodes.Event);
            w.Write(HandlerID);
            w.Write((byte)EventType.Aquire);
            w.Write(PlayerName.text);

            MatchServer.SendTo(w, MatchServer.GetConnection(cid));
        }

        private void SendNameEvent()
        {
            var w = new DataWriter(256);
            w.Write((byte)NetCodes.Event);
            w.Write(HandlerID);
            w.Write((byte)EventType.NameChanged);
            w.Write(PlayerName.text);

            MatchServer.Send(w);
        }

        private bool lrs;
        private int ltd;
        private int lts;
        public void Update()
        {
            if (lrs != ReadyToggle.isOn)
            {
                lrs = ReadyToggle.isOn;
                SendReadyEvent();
            }
            if (ltd != TypeDropdown.value)
            {
                ltd = TypeDropdown.value;
                SendTypeChangedEvent();
            }
            if (lts != TeamDropdown.value)
            {
                lts = TeamDropdown.value;
                SendTeamChangedEvent();
            }
        }

        private void SendReadyEvent()
        {
            var w = new DataWriter(16);
            w.Write((byte)NetCodes.Event);
            w.Write(HandlerID);
            w.Write((byte)(lrs ? EventType.ReadyOn : EventType.ReadyOff));

            if (GameLogic.IsHost)
                MatchServer.Send(w);
            else
                MatchClient.Send(w);
        }

        private void SendTypeChangedEvent()
        {
            var w = new DataWriter(16);
            w.Write((byte)NetCodes.Event);
            w.Write(HandlerID);
            w.Write((byte)EventType.TypeChanged);
            w.Write(TypeDropdown.value);

            if (GameLogic.IsHost)
                MatchServer.Send(w);
            else
                MatchClient.Send(w);
        }

        private void SendTeamChangedEvent()
        {
            var w = new DataWriter(16);
            w.Write((byte)NetCodes.Event);
            w.Write(HandlerID);
            w.Write((byte)EventType.TeamChanged);
            w.Write(TeamDropdown.value);

            if (GameLogic.IsHost)
                MatchServer.Send(w);
            else
                MatchClient.Send(w);
        }

        public void SyncAll()
        {
            SendNameEvent();
            SendReadyEvent();
            SendTypeChangedEvent();
            SendTeamChangedEvent();
        }

        private enum EventType : byte
        {
            ERROR = 0,
            ReadyOn,
            ReadyOff,
            TypeChanged,
            TeamChanged,
            NameChanged,
            Aquire
        }
    }
}
