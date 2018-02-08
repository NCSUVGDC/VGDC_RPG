using System;
using UnityEngine;
using UnityEngine.UI;

namespace VGDC_RPG.UI {
    public class PlayerLobbySettings {
        public Text PlayerName;
        public Dropdown TypeDropdown;
        public Dropdown TeamDropdown;
        public Toggle ReadyToggle;
        public GameObject pip;

        private static int nid = -20;

        public int CID { get; private set; }

        public int HandlerID { get; private set; }


        public PlayerLobbySettings(int i, int cid) {

            CID = cid; // experimenting with different Character (Team) IDs

            pip = UnityEngine.Object.Instantiate(Resources.Load("PlayerInfoPanel") as GameObject);
            PlayerName = pip.transform.Find("Text").GetComponent<Text>();
            TypeDropdown = pip.transform.Find("Dropdown").GetComponent<Dropdown>();
            TeamDropdown = pip.transform.Find("TeamDropdown").GetComponent<Dropdown>();
            ReadyToggle = pip.transform.Find("Toggle").GetComponent<Toggle>();

            ReadyToggle.isOn = false;

            HandlerID = nid--;

            pip.GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(0, -50 * i);
        }


        public void SetState(bool mine) {

            // Originally, all values were true
            TypeDropdown.interactable = mine;
            ReadyToggle.interactable = mine;
            TeamDropdown.interactable = mine;
        }
    }
}