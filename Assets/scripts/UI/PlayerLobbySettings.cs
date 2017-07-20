using System;
using UnityEngine;
using UnityEngine.UI;

namespace VGDC_RPG.UI
{
    public class PlayerLobbySettings
    {
        public Text PlayerName;
        public Dropdown TypeDropdown;
        public Dropdown TeamDropdown;
        public Toggle ReadyToggle;
        public GameObject pip;

        private static int nid = -20;

        public int CID { get; private set; }

        public int HandlerID { get; private set; }

        public PlayerLobbySettings(int i)
        {
            CID = -1;

            pip = UnityEngine.Object.Instantiate(Resources.Load("PlayerInfoPanel") as GameObject);
            PlayerName = pip.transform.FindChild("Text").GetComponent<Text>();
            TypeDropdown = pip.transform.FindChild("Dropdown").GetComponent<Dropdown>();
            TeamDropdown = pip.transform.FindChild("TeamDropdown").GetComponent<Dropdown>();
            ReadyToggle = pip.transform.FindChild("Toggle").GetComponent<Toggle>();

            ReadyToggle.isOn = false;

            HandlerID = nid--;

            pip.GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(0, -50 * i);
        }

        public void SetState(bool mine)
        {
            TypeDropdown.interactable = true;
            ReadyToggle.interactable = true;
            TeamDropdown.interactable = true;
        }
    }
}
