using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using VGDC_RPG.Networking;

namespace VGDC_RPG.UI
{
    public class PlayerLobbySettings : INetEventHandler
    {
        public Text PlayerName;
        public Dropdown TypeDropdown;
        public Toggle ReadyToggle;
        public GameObject pip;

        private static int nid = -2;

        public int HandlerID { get; private set; }

        public PlayerLobbySettings(GameObject content, int i)
        {
            pip = UnityEngine.Object.Instantiate(Resources.Load("PlayerInfoPanel") as GameObject);
            pip.transform.SetParent(content.transform);
            PlayerName = pip.transform.FindChild("Text").GetComponent<Text>();
            TypeDropdown = pip.transform.FindChild("Dropdown").GetComponent<Dropdown>();
            ReadyToggle = pip.transform.FindChild("Toggle").GetComponent<Toggle>();

            HandlerID = nid--;

            pip.GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(0, -50 * i);

            content.GetComponent<RectTransform>().sizeDelta = new Vector2(content.GetComponent<RectTransform>().sizeDelta.x, i * 50 + 50);

            NetEvents.RegisterHandler(this);

            UnityEngine.Debug.Log("HERE");
        }

        public void HandleEvent(DataReader r)
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
                    default:
                        throw new Exception("Invalid event type: " + et.ToString());
                }
            }
        }

        public void SetState(bool mine)
        {
            if (GameLogic.Instance.IsHost)
            {
                TypeDropdown.interactable = true;
            }
            if (mine)
                ReadyToggle.interactable = true;
        }

        private bool lrs;
        private int ltd;
        public void Update()
        {
            if (lrs != ReadyToggle.isOn)
            {
                lrs = ReadyToggle.isOn;

                var w = new DataWriter(16);
                w.Write((byte)NetCodes.Event);
                w.Write(HandlerID);
                w.Write((byte)(lrs ? EventType.ReadyOn : EventType.ReadyOff));

                if (GameLogic.Instance.IsHost)
                    MatchServer.Send(w);
                else
                    MatchClient.Send(w);
            }
            if (ltd != TypeDropdown.value)
            {
                ltd = TypeDropdown.value;

                var w = new DataWriter(16);
                w.Write((byte)NetCodes.Event);
                w.Write(HandlerID);
                w.Write((byte)EventType.TypeChanged);
                w.Write(TypeDropdown.value);

                if (GameLogic.Instance.IsHost)
                    MatchServer.Send(w);
                else
                    MatchClient.Send(w);
            }
        }

        private enum EventType : byte
        {
            ERROR = 0,
            ReadyOn,
            ReadyOff,
            TypeChanged
        }
    }
}
